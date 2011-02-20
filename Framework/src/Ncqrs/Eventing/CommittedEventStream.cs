using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs.Eventing
{
    public class CommittedEventStream : IEnumerable<CommittedEvent>
    {
        private readonly long _currentSourceVersion;
        private readonly Guid _sourceId;
        private readonly List<CommittedEvent> _events = new List<CommittedEvent>();
        
        public CommittedEventStream()
        {            
        }

        public CommittedEventStream(IEnumerable<CommittedEvent> events)
        {
            _events.AddRange(events);
            if (_events.Count > 0)
            {
                var last = _events.Last();
                _currentSourceVersion = last.EventSequence;
                _sourceId = last.EventSourceId;
            }
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
            get { return _currentSourceVersion; }
        }

        public IEnumerator<CommittedEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}