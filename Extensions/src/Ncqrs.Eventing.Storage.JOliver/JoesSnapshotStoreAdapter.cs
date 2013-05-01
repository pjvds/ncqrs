using System;
using EventStore;
using Snapshot = Ncqrs.Eventing.Sourcing.Snapshotting.Snapshot;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JoesSnapshotStoreAdapter : ISnapshotStore
    {
        private readonly IAccessSnapshots _snapshotAccessor;

        public JoesSnapshotStoreAdapter(IAccessSnapshots snapshotAccessor)
        {
            _snapshotAccessor = snapshotAccessor;
        }

        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            int maxRevision = maxVersion == long.MaxValue ? int.MaxValue : (int) maxVersion;
            var result = _snapshotAccessor.GetSnapshot(eventSourceId, maxRevision);

            if (result != null)
                return new Snapshot(result.StreamId, result.StreamRevision, result.Payload);

            return null;
        }

        public void SaveSnapshot(Snapshot snapshot)
        {
            _snapshotAccessor.AddSnapshot(new EventStore.Snapshot(snapshot.EventSourceId, (int)snapshot.Version, snapshot.Payload));
        }
    }
}
