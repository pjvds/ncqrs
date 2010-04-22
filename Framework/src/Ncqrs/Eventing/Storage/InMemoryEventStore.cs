using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// An in memory event store that can be used for unit testing purpose. We can't
    /// think of any situation where you want to use this in production.
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private readonly Dictionary<Guid, LinkedList<IEvent>> _events = new Dictionary<Guid, LinkedList<IEvent>>();

        public IEnumerable<IEvent> GetAllEventsForEventSource(Guid id)
        {
            LinkedList<IEvent> events;

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
            LinkedList<IEvent> events;
            var eventsToCommit = source.GetUncommitedEvents();

            if (!_events.TryGetValue(source.Id, out events))
            {
                events = new LinkedList<IEvent>();
                _events.Add(source.Id, events);
            }

            foreach (var evnt in eventsToCommit)
            {
                events.AddLast(evnt);
            }

            return eventsToCommit;
        }
    }
}
