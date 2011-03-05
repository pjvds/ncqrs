using System;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot;

namespace System.Linq
{
    public static class DynamicSnapshotExtensions
    {
        public static void InitializeFrom(this DynamicSnapshotBase snapshot, AggregateRoot source)
        {
            DynamicSnapshot.InitializeFrom(snapshot, source);
        }

        public static void RestoreAggregateRoot(this DynamicSnapshotBase snapshot, AggregateRoot source)
        {
            DynamicSnapshot.RestoreAggregateRoot(snapshot, source);
        }
    }
}