using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Domain;
using Castle.DynamicProxy;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class DynamicSnapshot
    {
        private static Assembly _snapshotAssembly;

        public static object Create(Type aggregateType)
        {
            var snapshotable = CreateSnapshotable(aggregateType);
            var generator = new ProxyGenerator();

            var options = new ProxyGenerationOptions();
            options.AddMixinInstance(snapshotable);

            var proxy = generator.CreateClassProxy(aggregateType, options);
            ((IHaveProxyReference)proxy).Proxy = proxy;

            return proxy;
        }

        public static T Create<T>() where T : AggregateRoot
        {
            var snapshotable = CreateSnapshotable<T>();
            var generator = new ProxyGenerator();

            var options = new ProxyGenerationOptions();
            options.AddMixinInstance(snapshotable);

            var proxy = generator.CreateClassProxy(typeof(T), options);
            ((IHaveProxyReference)proxy).Proxy = proxy;

            return (T)proxy;
        }

        /// <summary>
        /// Creates an assembly with snapshot types.
        /// </summary>
        /// <param name="target">The assembly containing aggregate roots with [DynamicSnapshot] attribute.</param>
        /// <returns></returns>
        public static Assembly CreateAssemblyFrom(Assembly target)
        {
            var snapshotableTypes = target.GetTypes().Where(type => type.HasAttribute<DynamicSnapshotAttribute>());

            if (snapshotableTypes.Count() == 0)
                throw new DynamicSnapshotException(string.Format(
                    "The assembly '{0}' does not contain any snapshotable types. Are you missing [{1}] attribute?",
                    target.FullName,
                    typeof(DynamicSnapshotAttribute).Name));

            var snapshotAssemblyBuilder = new DynamicSnapshotAssemblyBuilder();

            foreach (var type in snapshotableTypes)
                snapshotAssemblyBuilder.RegisterSnapshotType(type);

            _snapshotAssembly = snapshotAssemblyBuilder.SaveAssembly();

            return _snapshotAssembly;
        }

        internal static T CreateSnapshotable<T>() where T : AggregateRoot
        {
            return (T)CreateSnapshotable(typeof(T));
        }

        internal static object CreateSnapshotable(Type aggregateType)
        {
            var snapshotType = FindSnapshotType(aggregateType);
            var aggregateTypeName = aggregateType.Name;
            var snapshotTypeName = snapshotType.Name;

            //  May be we should log a warning. The exception is a bit rude.
            if (!snapshotTypeName.StartsWith(aggregateTypeName))
                throw new DynamicSnapshotException(string.Format("Invalid snapshot [{0}]' for type [{1}].", snapshotTypeName, aggregateTypeName));

            var snapshotableType = typeof(SnapshotableImplementer<>).MakeGenericType(snapshotType);
            return Activator.CreateInstance(snapshotableType);
        }

        private static Type FindSnapshotType<T>() where T : AggregateRoot
        {
            return FindSnapshotType(typeof(T));
        }

        private static Type FindSnapshotType(Type aggregateType)
        {
            LoadSnapshotAssembly();
            var aggregateTypeName = aggregateType.Name;
            var snapshotType = _snapshotAssembly.GetTypes().SingleOrDefault(type => type.Name.StartsWith(aggregateTypeName));

            if (snapshotType == null)
                throw new DynamicSnapshotException(string.Format(
                    "Cannot find snapshot in '{0}' for type [{1}]. Consider rebuilding the dynamic snapshot assembly.",
                    _snapshotAssembly.FullName,
                    aggregateTypeName));

            return snapshotType;
        }

        private static Assembly LoadSnapshotAssembly()
        {
            if (_snapshotAssembly == null)
            {
                try
                {
                    _snapshotAssembly = Assembly.LoadFrom(DynamicSnapshotAssemblyBuilder.DefaultModuleName);
                }
                catch (Exception ex)
                {
                    throw new DynamicSnapshotException("See inner exception for details.", ex);
                }
            }
            return _snapshotAssembly;
        }

    }
}