using System;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Domain.Storage
{
    public class AggregateSupportsSnapshotValidator : IAggregateSupportsSnapshotValidator
    {
        public bool DoesAggregateSupportsSnapshot(Type aggregateType, Type snapshotType)
        {
            var memType = aggregateType.GetSnapshotInterfaceType();

            var expectedType = typeof(ISnapshotable<>).MakeGenericType(snapshotType);
            return memType == expectedType;
        }
    }
}
