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

        public CommittedEventStream(Guid sourceId, params CommittedEvent[] events)
            : this (sourceId, (IEnumerable<CommittedEvent>)events)
        {
        }

        public CommittedEventStream(Guid sourceId, IEnumerable<CommittedEvent> events)
        {
            _sourceId = sourceId;
            _events.AddRange(events);
            if (_events.Count > 0)
            {
                var last = _events.Last();
                _currentSourceVersion = last.EventSequence;
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