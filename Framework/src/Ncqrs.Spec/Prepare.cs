using System;
using System.Collections.Generic;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Spec
{
    public static class Prepare
    {
        public class PrepareTheseEvents
        {
            private readonly IEnumerable<object> _events;

            public PrepareTheseEvents(IEnumerable<object> events)
            {
                _events = events;
            }

            public CommittedEventStream ForSource(Guid id, int sequenceOffset = 0)
            {
                int sequence = sequenceOffset + 1;

                var commitId = Guid.NewGuid();
                var comittedEvents = new List<CommittedEvent>();
                foreach (var evnt in _events)
                {
                    var committedEvent = new CommittedEvent(commitId, Guid.NewGuid(), id, sequence, DateTime.UtcNow,
                                                            evnt, new Version(1, 0));
                    sequence++;
                    comittedEvents.Add(committedEvent);
                }
                return new CommittedEventStream(comittedEvents);
            }
        }

        public static PrepareTheseEvents Events(IEnumerable<object> events)
        {
            return new PrepareTheseEvents(events);
        }

        public static PrepareTheseEvents Events(params object[] events)
        {
            return new PrepareTheseEvents(events);
        }
    }
}
