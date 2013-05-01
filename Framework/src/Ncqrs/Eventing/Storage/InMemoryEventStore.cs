using System;
using System.Collections.Generic;
using System.Linq;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>An in memory event store that can be used for unit testing purpose. We can't
    /// think of any situation where you want to use this in production.</summary>
    public class InMemoryEventStore : IEventStore, ISnapshotStore
    {
        private readonly Dictionary<Guid, Queue<CommittedEvent>> _events = new Dictionary<Guid, Queue<CommittedEvent>>();
        private readonly Dictionary<Guid, Snapshot> _snapshots = new Dictionary<Guid, Snapshot>();

        /// <summary>Gets a snapshot of a particular event source, if one exists. Otherwise, returns <c>null</c>.</summary>
        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            var result = _snapshots[eventSourceId];

            return result.Version > maxVersion ? null : result;
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion)
        {
            Queue<CommittedEvent> events;

            if (_events.TryGetValue(id, out events))
            {
                var committedEvents = events
                    .Where(x => x.EventSequence >= minVersion && x.EventSequence <= maxVersion);
                return new CommittedEventStream(id, committedEvents);
            }
            return new CommittedEventStream(id);
        }

        /// <summary>Saves a snapshot of the specified event source.</summary>
        public void SaveSnapshot(Snapshot snapshot)
        {
            _snapshots[snapshot.EventSourceId] = snapshot;
        }

        public void Store(UncommittedEventStream eventStream)
        {
            Queue<CommittedEvent> events;
            if (eventStream.IsNotEmpty)
            {
                if (!_events.TryGetValue(eventStream.SourceId, out events))
                {
                    events = new Queue<CommittedEvent>();
                    _events.Add(eventStream.SourceId, events);
                }

                foreach (var evnt in eventStream)
                {
                    events.Enqueue(new CommittedEvent(eventStream.CommitId, evnt.EventIdentifier, eventStream.SourceId, evnt.EventSequence,
                                                      evnt.EventTimeStamp, evnt.Payload, evnt.EventVersion));
                }
            }
        }
    }
}
