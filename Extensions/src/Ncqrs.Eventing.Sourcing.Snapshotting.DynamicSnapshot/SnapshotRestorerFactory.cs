using System;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    internal static class SnapshotRestorerFactory
    {
        public static ISnapshotRestorer Create(AggregateRoot aggregateRoot, object snapshot)
        {
            var snapshotType = snapshot.GetType();
            if (!typeof(DynamicSnapshotBase).IsAssignableFrom(snapshotType)) throw new ArgumentException("snapshot must inherit DynamicSnapshotBase");
            var restorerType = typeof(SnapshotRestorer<>).MakeGenericType(snapshot.GetType());
            return (ISnapshotRestorer)Activator.CreateInstance(restorerType, aggregateRoot, snapshot);
        }
    }
}
