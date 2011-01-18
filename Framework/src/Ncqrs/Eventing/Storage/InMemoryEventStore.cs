using System;
using System.Collections.Generic;
using System.Linq;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// An in memory event store that can be used for unit testing purpose. We can't
    /// think of any situation where you want to use this in production.
    /// </summary>
    public class InMemoryEventStore : IEventStore, ISnapshotStore
    {
        private readonly Dictionary<Guid, Queue<CommittedEvent>> _events = new Dictionary<Guid, Queue<CommittedEvent>>();
        private readonly Dictionary<Guid, ISnapshot> _snapshots = new Dictionary<Guid, ISnapshot>();
        
        /// <summary>
        /// Saves a snapshot of the specified event source.
        /// </summary>
        public void SaveShapshot(ISnapshot snapshot)
        {
            _snapshots[snapshot.EventSourceId] = snapshot;
        }

        /// <summary>
        /// Gets a snapshot of a particular event source, if one exists. Otherwise, returns <c>null</c>.
        /// </summary>
        public ISnapshot GetSnapshot(Guid eventSourceId)
        {
            return _snapshots[eventSourceId];
        }

        public CommittedEventStream ReadUntil(Guid id, long? maxVersion)
        {
            Queue<CommittedEvent> events;

            if (_events.TryGetValue(id, out events))
            {
                var committedEvents = events
                    .Where(x => !maxVersion.HasValue || x.EventSequence <= maxVersion.Value);
                return new CommittedEventStream(committedEvents);
            }
            return new CommittedEventStream();
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion)
        {
            Queue<CommittedEvent> events;
            
            if (_events.TryGetValue(id, out events))
            {
                var committedEvents = events
                    .Where(x => x.EventSequence >= minVersion);                    
                return new CommittedEventStream(committedEvents);
            }
            return new CommittedEventStream();
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
                                                      evnt.EventTimeStamp, evnt.Payload));
                }
            }
        }
    }
}
