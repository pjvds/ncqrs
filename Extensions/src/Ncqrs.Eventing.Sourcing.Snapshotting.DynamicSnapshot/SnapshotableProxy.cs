using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using System.Reflection;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class SnapshotableProxy
    {
        #region Public Methods

        private static T Create<T>(T aggreagte, Type snapshotType) where T : AggregateRoot
        {
            var aggregateTypeName = typeof(T).Name;
            var snapshotTypeName = snapshotType.Name;

            //  May be we should log a warning. The exception is a bit rude.
            if (!snapshotTypeName.StartsWith(aggregateTypeName))
                throw new DynamicSnapshotException(string.Format("Invalid snapshot [{0}]' for type [{1}].", snapshotTypeName, aggregateTypeName));

            var snapshotableType = typeof(SnapshotableImplementer<>).MakeGenericType(snapshotType);
            var snapshotable = Activator.CreateInstance(snapshotableType);
            var generator = new ProxyGenerator();

            var options = new ProxyGenerationOptions();
            options.AddMixinInstance(snapshotable);

            var proxy = generator.CreateClassProxyWithTarget(typeof(T), aggreagte, options);
            ((IHaveProxyReference)proxy).Proxy = (AggregateRoot)proxy;

            return (T)proxy;
        }

        public static T Create<T>(T aggregate) where T : AggregateRoot
        {
            var snapshotType = DynamicSnapshot.FindSnapshotType(aggregate);
            return Create(aggregate, snapshotType);
        }

        #endregion

    }
}
