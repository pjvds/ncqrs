using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Collections;

namespace Ncqrs.Eventing.Sourcing
{
    public class SourcedEventStream : IEnumerable<SourcedEvent>
    {
        private Guid _eventSourceId;
        private long _sequence;

        private List<SourcedEvent> _events = new List<SourcedEvent>();

        public Guid EventSourceId
        {
            get
            {
                return _eventSourceId;
            }
            set
            {
                Contract.Requires<InvalidOperationException>(_events.Count == 0);
                _eventSourceId = value;
            }
        }

        public long Sequence
        {
            get
            {
                return _sequence;
            }
            set
            {
                Contract.Requires<InvalidOperationException>(_events.Count == 0);
                _sequence = value;                
            }
        }

        public int Count
        {
            get
            {
                return _events.Count;
            }
        }

        public SourcedEventStream()
        {
        }

        public SourcedEventStream(Guid eventSourceId, long sequence)
        {
            _eventSourceId = eventSourceId;
            _sequence = sequence;
        }

        [ContractInvariantMethod]
        protected void ContractInvariants()
        {
            Contract.Invariant(_eventSourceId != Guid.Empty);
            Contract.Invariant(_sequence >= 0);
            Contract.Invariant(Contract.ForAll(_events, (sourcedEvent) => sourcedEvent.EventSourceId == _eventSourceId));
        }

        public void Append(IEvent evnt)
        {
            var sourcedEvent = new SourcedEvent(_eventSourceId, _sequence++, evnt);
            _events.Add(sourcedEvent);
        }

        public void Clear()
        {
            _events.Clear();
        }

        public IEnumerator<SourcedEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
