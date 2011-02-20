using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ncqrs.Eventing
{
    public class CommittedEventStream : IEnumerable<CommittedEvent>
    {
        private readonly long _fromVersion;
        private readonly long _toVersion;
        private readonly Guid _sourceId;
        private readonly List<CommittedEvent> _events = new List<CommittedEvent>();

        public long FromVersion
        {
            get { return _fromVersion; }
        }

        public long ToVersion
        {
            get { return _toVersion; }
        }

        public bool IsEmpty
        {
            get { return _events.Count == 0; }
        }

        public Guid SourceId
        {
            get { return _sourceId; }
        }

        public long CurrentSourceVersion
        {
            get { return _toVersion; }
        }

        public IEnumerator<CommittedEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public CommittedEventStream(Guid sourceId)
        {
            _sourceId = sourceId;
        }

        public CommittedEventStream(Guid sourceId, params CommittedEvent[] events)
            : this(sourceId, (IEnumerable<CommittedEvent>)events)
        {
        }

        public CommittedEventStream(Guid sourceId, IEnumerable<CommittedEvent> events)
        {
            _sourceId = sourceId;

            if(events != null) _events = new List<CommittedEvent>(events);

            ValidateEventInformation(_events);
            
            if (_events.Count > 0)
            {
                var first = _events.First();
                _fromVersion = first.EventSequence;

                var last = _events.Last();
                _toVersion = last.EventSequence;

                _toVersion = _events.OrderByDescending(evnt => evnt.EventSequence).First().EventSequence;
            }
        }

        private void ValidateEventInformation(IEnumerable<CommittedEvent> events)
        {
            // An empty event stream is allowed.
            if (events == null || events.IsEmpty()) return;

            var firstEvent = events.First();
            var startSequence = firstEvent.EventSequence;

            var expectedSourceId = _sourceId;
            var expectedSequence = startSequence;

            ValidateSingleEvent(firstEvent, 0, expectedSourceId, expectedSequence);

            for(int position = 1; position < _events.Count; position++)
            {
                var evnt = _events[position];
                expectedSourceId = _sourceId;
                expectedSequence = startSequence + position;

                ValidateSingleEvent(evnt, position, expectedSourceId, expectedSequence);
            }
        }

        private void ValidateSingleEvent(CommittedEvent evnt, long position, Guid expectedSourceId, long expectedSequence)
        {
            if (evnt == null)
                throw new ArgumentNullException("events", "The events stream contains a null reference at position " + position + ".");

            if (evnt.EventSourceId != expectedSourceId)
            {
                var msg = string.Format("The events stream contains an event that is related to another event " +
                                        "source at position {0}. Expected event source id {1}, but actual was {2}",
                                        position, _sourceId, evnt.EventSourceId);
                throw new ArgumentException("events", msg);
            }

            if (evnt.EventSequence != expectedSequence)
            {
                var msg =
                    string.Format("The events stream contains an committed event with an illigal sequence at " +
                                  "position {0}. The expected sequence is {1}, but actual was {2}.",
                                  position, expectedSequence, evnt.EventSequence);

                throw new ArgumentException("events", msg);
            }
        }

        //    if(events.IsEmpty())
        //        return;

        //    var first = events.First();

        //    var eventCounter = 0;
        //    var eventSourceId = first.EventSourceId;
        //    var currentEventSequence = first.EventSequence;

        //    foreach(var evnt in events.Skip(1))
        //    {
        //        if(evnt.EventSourceId != eventSourceId)
        //        {
        //            var msg =
        //                String.Format("Event {0} did not contain the expected event source id {1}. "+
        //                    "Actual value was {2}.",
        //                    evnt.EventIdentifier, eventSourceId, evnt.EventSourceId);
        //            throw new ArgumentOutOfRangeException("events", msg);
        //        }
        //}
    }

    [Serializable]
    public class InvalidCommittedEventException : Exception
    {
        public InvalidCommittedEventException()
        {
        }

        public InvalidCommittedEventException(string message) : base(message)
        {
        }

        public InvalidCommittedEventException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidCommittedEventException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            
        }
    }
}