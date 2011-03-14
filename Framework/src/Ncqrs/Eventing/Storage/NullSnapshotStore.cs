using System;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage
{
    public class NullSnapshotStore : ISnapshotStore
    {
        public void SaveShapshot(Snapshot source)
        {
        }

        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            return null;
        }
    }
}