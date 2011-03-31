using System;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing;

namespace Ncqrs.Domain.Storage
{
    public interface IAggregateSnapshotter
    {
        bool TryLoadFromSnapshot(Type aggregateRootType, Snapshot snapshot, CommittedEventStream committedEventStream, out AggregateRoot aggregateRoot);
        bool TryTakeSnapshot(AggregateRoot aggregateRoot, out Snapshot snapshot);
    }
}
