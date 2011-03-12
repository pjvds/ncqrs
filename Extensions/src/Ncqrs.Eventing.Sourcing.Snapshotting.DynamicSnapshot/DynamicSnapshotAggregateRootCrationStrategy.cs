using System;
using Ncqrs.Domain.Storage;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public class DynamicSnapshotAggregateRootCrationStrategy : IAggregateRootCreationStrategy
    {
        public T CreateAggregateRoot<T>() where T : Domain.AggregateRoot
        {
            return DynamicSnapshot.Create<T>();
        }

        public Domain.AggregateRoot CreateAggregateRoot(Type aggregateRootType)
        {
            return (Domain.AggregateRoot)DynamicSnapshot.Create(aggregateRootType);
        }
    }
}
