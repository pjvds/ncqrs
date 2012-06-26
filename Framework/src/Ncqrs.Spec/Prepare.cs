using System;
using System.Collections.Generic;
using System.Linq;
using Ncqrs.Eventing;

namespace Ncqrs.Spec
{
    public static class Prepare
    {
        public static PrepareTheseEvents Events(IEnumerable<object> events)
        {
            return new PrepareTheseEvents(events);
        }

        public static PrepareTheseEvents Events(params object[] events)
        {
            return new PrepareTheseEvents(events);
        }

        #region Nested type: PrepareTheseEvents

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

                Guid commitId = Guid.NewGuid();
                var comittedEvents = new List<CommittedEvent>();
                foreach (object evnt in _events)
                {
                    var committedEvent = new CommittedEvent(commitId, Guid.NewGuid(), id, sequence, DateTime.UtcNow,
                                                            evnt, new Version(1, 0));
                    sequence++;
                    comittedEvents.Add(committedEvent);
                }
                return new CommittedEventStream(id, comittedEvents);
            }

            public UncommittedEventStream ForSourceUncomitted(Guid id, Guid commitId, int sequenceOffset = 0)
            {
                int initialVersion = sequenceOffset == 0 ? 1 : sequenceOffset;
                int sequence = initialVersion;

                var result = new UncommittedEventStream(commitId);
                foreach (UncommittedEvent uncommittedEvent
                    in _events.Select(evnt =>
                                      new UncommittedEvent(Guid.NewGuid(),
                                                           id, sequence, initialVersion,
                                                           DateTime.UtcNow,
                                                           evnt, new Version(1, 0))))
                {
                    result.Append(uncommittedEvent);
                    sequence++;
                }
                return result;
            }
        }

        #endregion
    }
}