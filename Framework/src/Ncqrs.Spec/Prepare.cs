using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Spec
{
    public static class Prepare
    {
        public class PrepareTheseEvents<TEvent> where TEvent : SourcedEvent
        {
            private readonly IEnumerable<TEvent> _events;

            public PrepareTheseEvents(IEnumerable<TEvent> events)
            {
                _events = events;
            }

            public IEnumerable<TEvent> ForSource(Guid id, int sequenceOffset = 0)
            {
                int sequence = sequenceOffset+1;

                foreach(var evnt in _events)
                {
                    evnt.EventSourceId = id;
                    evnt.EventSequence = sequence;

                    sequence++;

                    yield return evnt;
                }
            }
        }

        public static PrepareTheseEvents<SourcedEvent> Events(IEnumerable<SourcedEvent> events)
        {
            return new PrepareTheseEvents<SourcedEvent>(events);
        }

        public static PrepareTheseEvents<SourcedEvent> Events(params SourcedEvent[] events)
        {
            return new PrepareTheseEvents<SourcedEvent>(events);
        }
    }
}
