using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Domain;
using Castle.DynamicProxy;
using System.Runtime.InteropServices;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    /// <summary>
    /// Provides a way to create an assembly with snapshot types and finder for snapshot types.
    /// </summary>
    internal class DynamicSnapshotAssembly : IDynamicSnapshotAssembly
    {
        private readonly DynamicSnapshotAssemblyBuilder _assemblyBuidler;

        private Assembly _snapshotAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicSnapshotAssembly"/> class.
        /// </summary>
        /// <param name="assemblyBuilder">The assembly builder.</param>
        public DynamicSnapshotAssembly(DynamicSnapshotAssemblyBuilder assemblyBuilder)
        {
            _assemblyBuidler = assemblyBuilder;
        }

        /// <summary>
        /// Gets the actual assembly.
        /// </summary>
        public Assembly ActualAssembly { get { return LoadSnapshotAssembly(); } }

        /// <summary>
        /// Creates an assembly with snapshot types.
        /// </summary>
        /// <param name="target">The assembly containing aggregate roots with [DynamicSnapshot] attribute.</param>
        /// <returns></returns>
        public Assembly CreateAssemblyFrom(Assembly target)
        {
            var snapshotableTypes = target.GetTypes().Where(type => type.HasAttribute<DynamicSnapshotAttribute>());

            if (snapshotableTypes.Count() == 0)
                throw new DynamicSnapshotException(string.Format(
                    "The assembly '{0}' does not contain any snapshotable types. Are you missing [{1}] attribute?",
                    target.FullName,
                    typeof(DynamicSnapshotAttribute).Name));

            foreach (var type in snapshotableTypes)
                _assemblyBuidler.RegisterSnapshotType(type);

            _snapshotAssembly = _assemblyBuidler.SaveAssembly();

            return _snapshotAssembly;
        }

        /// <summary>
        /// Finds a snapshot type.
        /// </summary>
        /// <param name="aggregateType">Type of the aggregate.</param>
        /// <returns></returns>
        public Type FindSnapshotType(Type aggregateType)
        {
            LoadSnapshotAssembly();

            var aggregateTypeName = aggregateType.Name + "_Snapshot";
            var snapshotType = _snapshotAssembly.GetTypes().SingleOrDefault(type => type.Name.StartsWith(aggregateTypeName));

            if (snapshotType == null)
                throw new DynamicSnapshotException(string.Format(
                    "Cannot find snapshot in '{0}' for type [{1}]. Consider rebuilding the dynamic snapshot assembly.",
                    _snapshotAssembly.FullName,
                    aggregateTypeName));

            return snapshotType;
        }

        /// <summary>
        /// Finds a snapshot type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Type FindSnapshotType<T>()
                where T : AggregateRoot
        {
            return FindSnapshotType(typeof(T));
        }

        private Assembly LoadSnapshotAssembly()
        {
            if (_snapshotAssembly == null)
            {
                try
                {
                    _snapshotAssembly = Assembly.LoadFrom(DynamicSnapshotAssemblyBuilder.DefaultModuleName);
                }
                catch (Exception ex)
                {
                    throw new DynamicSnapshotException("Cannot find 'DynamicSnapshot.dll' assembly. Did you forget to initialize the DynamicSnapshot facility?", ex);
                }
            }
            return _snapshotAssembly;
        }

        public string AssemblyGuidString(System.Reflection.Assembly assembly)
        {
            object[] objects = assembly.GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
            if (objects.Length > 0)
            {
                return ((System.Runtime.InteropServices.GuidAttribute)objects[0]).Value;
            }
            else
            {
                return String.Empty;
            }
        }

    }
}