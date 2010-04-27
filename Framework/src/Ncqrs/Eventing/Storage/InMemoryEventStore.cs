using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// An in memory event store that can be used for unit testing purpose. We can't
    /// think of any situation where you want to use this in production.
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private readonly Dictionary<Guid, Queue<IEvent>> _events = new Dictionary<Guid, Queue<IEvent>>();

        public IEnumerable<IEvent> GetAllEventsForEventSource(Guid id)
        {
            Queue<IEvent> events;

            if (_events.TryGetValue(id, out events))
            {
                foreach (var evnt in events)
                {
                    yield return evnt;
                }
            }
        }

        public IEnumerable<IEvent> Save(IEventSource source)
        {
            Queue<IEvent> events;
            var eventsToCommit = source.GetUncommittedEvents();

            if (!_events.TryGetValue(source.Id, out events))
            {
                events = new Queue<IEvent>();
                _events.Add(source.Id, events);
            }

            foreach (var evnt in eventsToCommit)
            {
                events.Enqueue(evnt);
            }

            // TODO: .net 4.0 co/con
            return eventsToCommit.Cast<IEvent>();
        }
    }
}
