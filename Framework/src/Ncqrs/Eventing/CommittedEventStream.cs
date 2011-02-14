using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public CommittedEventStream()
        {
        }

        public CommittedEventStream(IEnumerable<CommittedEvent> events)
        {
            // TODO: ValidateEventInformation(events);

            if (_events.Count > 0)
            {
                var first = _events.First();
                _fromVersion = first.EventSequence;

                var last = _events.Last();
                _toVersion = last.EventSequence;

                _toVersion = _events.OrderByDescending(evnt => evnt.EventSequence).First().EventSequence;
                _sourceId = last.EventSourceId;
            }
        }

        //private static void ValidateEventInformation(IEnumerable<CommittedEvent> events)
        //{
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

        public static CommittedEventStream Combine(IEnumerable<CommittedEventStream> streams)
        {
            var events = new List<CommittedEvent>();

            foreach (var stream in streams.OrderBy(strm=>strm._fromVersion))
            {
                events.AddRange(stream);
            }

            return new CommittedEventStream(events);
        }
    }
}