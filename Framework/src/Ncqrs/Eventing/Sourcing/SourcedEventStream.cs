using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace Ncqrs.Eventing.Sourcing
{
    public class SourcedEventStream : IEnumerable<ISourcedEvent>
    {
        private Guid _eventSourceId;
        private long _sequence;

        private IList<ISourcedEvent> _events = new List<ISourcedEvent>();

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

        public void Append(ISourcedEvent sourcedEvent)
        {
            // TODO: Validate sequence and source id.
            _events.Add(sourcedEvent);
        }

        public void Append(IEnumerable<ISourcedEvent> eventDatas)
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

        public IEnumerator<ISourcedEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
