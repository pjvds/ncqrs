using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// An in memory event store that can be used for unit testing purpose. We can't
    /// think of any situation where you want to use this in production.
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private readonly Dictionary<Guid, Queue<SourcedEvent>> _events = new Dictionary<Guid, Queue<SourcedEvent>>();
        private readonly Dictionary<Guid, ISnapshot> _snapshots = new Dictionary<Guid, ISnapshot>();

        public IEnumerable<SourcedEvent> GetAllEvents(Guid id)
        {
            Queue<SourcedEvent> events;

            if (_events.TryGetValue(id, out events))
            {
                foreach (var evnt in events)
                {
                    yield return evnt;
                }
            }
        }

        /// <summary>
        /// Get all events provided by an specified event source.
        /// </summary>
        /// <param name="eventSourceId">The id of the event source that owns the events.</param>
        /// <returns>All the events from the event source.</returns>
        public IEnumerable<SourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            Queue<SourcedEvent> events;

            if (_events.TryGetValue(id, out events))
            {
                foreach (var evnt in events)
                {
                    if (evnt.EventSequence > version)
                    {
                        yield return evnt;
                    }
                }
            }
        }

        public void Save(IEventSource source)
        {
            Queue<SourcedEvent> events;
            var eventsToCommit = source.GetUncommittedEvents();

            if (!_events.TryGetValue(source.EventSourceId, out events))
            {
                events = new Queue<SourcedEvent>();
                _events.Add(source.EventSourceId, events);
            }

            foreach (var evnt in eventsToCommit)
            {
                events.Enqueue(evnt);
            }
        }

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
    }
}
