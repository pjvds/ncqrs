using System;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage
{
    public class NullSnapshotStore : ISnapshotStore
    {
        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            return null;
        }

        public void SaveSnapshot(Snapshot source)
        {
        }
    }
}