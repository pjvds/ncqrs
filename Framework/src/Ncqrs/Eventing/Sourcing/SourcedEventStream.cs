using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Collections;

namespace Ncqrs.Eventing.Sourcing
{
    public class SourcedEventStream : IEnumerable<ISourcedEvent<IEventData>>
    {
        private Guid _eventSourceId;
        private long _sequence;

        private IList<ISourcedEvent<IEventData>> _events = new List<ISourcedEvent<IEventData>>();

        public Guid EventSourceId
        {
            get
            {
                return _eventSourceId;
            }
            set
            {
                Contract.Requires<InvalidOperationException>(IsEmpty);
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
                Contract.Requires<InvalidOperationException>(IsEmpty);
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

        public bool IsEmpty
        {
            get
            {
                return Count == 0;
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
            Contract.Invariant(_sequence >= 0);
            Contract.Invariant(Contract.ForAll(_events, (sourcedEvent) => sourcedEvent.EventSourceId == _eventSourceId));
        }

        public void Append(IEventData eventData)
        {
            // TODO: Move to factory.
            var sourcedEvent = SourcedEvent<IEventData>.Create(_eventSourceId, _sequence++, eventData);
            _events.Add(sourcedEvent);
        }

        public void Clear()
        {
            _events.Clear();
        }

        public IEnumerator<ISourcedEvent<IEventData>> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
