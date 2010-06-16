using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Collections;

namespace Ncqrs.Eventing.Sourcing
{
    public class SourcedEventStream : IEnumerable<SourcedEvent>
    {
        private Guid _eventSourceId;

        private long _sequenceOffset;

        private IList<SourcedEvent> _events = new List<SourcedEvent>();

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

        public long SequenceOffset
        {
            get
            {
                return _sequenceOffset;
            }
            set
            {
                Contract.Requires<InvalidOperationException>(IsEmpty);
                _sequenceOffset = value;                
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

        public SourcedEventStream(Guid eventSourceId, long sequenceOffset)
        {
            _eventSourceId = eventSourceId;
            _sequenceOffset = sequenceOffset;
        }

        [ContractInvariantMethod]
        protected void ContractInvariants()
        {
            Contract.Invariant(_sequenceOffset >= 0);
            Contract.Invariant(Contract.ForAll(_events, (sourcedEvent) => sourcedEvent.EventSourceId == _eventSourceId));
            Contract.Invariant(Contract.ForAll(_events, (sourcedEvent) => sourcedEvent.EventSequence == (_sequenceOffset+_events.IndexOf(sourcedEvent)+1)));
        }

        public void Append(SourcedEvent sourcedEvent)
        {
            // TODO: Validate sequence and source id.
            _events.Add(sourcedEvent);
        }

        public void Append(IEnumerable<SourcedEvent> eventDatas)
        {
            foreach(var data in eventDatas)
            {
                Append(data);
            }
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
