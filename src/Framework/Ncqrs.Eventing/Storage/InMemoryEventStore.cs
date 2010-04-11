using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// An in memory event store that can be used for unit testing purpose. We can't
    /// think of any situation where you want to use this in production.
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private readonly Dictionary<Guid, Stack<Tuple<DateTime, IEvent>>> _events = new Dictionary<Guid,Stack<Tuple<DateTime,IEvent>>>();

        public IEnumerable<HistoricalEvent> GetAllEventsForEventSource(Guid id)
        {
            Stack<Tuple<DateTime, IEvent>> events;

            if (_events.TryGetValue(id, out events))
            {
                foreach (var eventData in events)
                {
                    yield return new HistoricalEvent(eventData.Item1, eventData.Item2);
                }
            }
        }

        public IEnumerable<IEvent> Save(EventSource source)
        {
            Stack<Tuple<DateTime, IEvent>> events;
            var eventsToCommit = source.GetUncommitedEvents();

            if (!_events.TryGetValue(source.Id, out events))
            {
                events = new Stack<Tuple<DateTime,IEvent>>();
                _events.Add(source.Id, events);
            }

            foreach (var evnt in eventsToCommit)
            {
                events.Push(new Tuple<DateTime, IEvent>(DateTime.Now, evnt));
            }

            return eventsToCommit;
        }
    }
}
