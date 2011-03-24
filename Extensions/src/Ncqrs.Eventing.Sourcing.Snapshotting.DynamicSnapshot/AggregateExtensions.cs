using System;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public static class AggregateExtensions
    {
        public static bool RestoreFromSnapshot(this AggregateRoot aggregateRoot, object snapshot)
        {
            return SnapshotRestorerFactory.From(aggregateRoot, snapshot).Restore();
        }
    }
}
