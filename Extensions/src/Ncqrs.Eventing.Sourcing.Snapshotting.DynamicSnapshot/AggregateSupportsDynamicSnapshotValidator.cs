using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Storage;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public class AggregateSupportsDynamicSnapshotValidator : IAggregateSupportsSnapshotValidator
    {
        public bool DoesAggregateSupportsSnapshot(Type aggregateType, Type snapshotType)
        {
            bool hasAttribute = aggregateType.HasAttribute<DynamicSnapshotAttribute>();
            bool doesSupportSnapshot = snapshotType.Name == SnapshotNameGenerator.Generate(aggregateType);

            return hasAttribute && doesSupportSnapshot;
        }
    }
}
