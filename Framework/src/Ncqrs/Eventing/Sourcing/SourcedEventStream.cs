using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Globalization;
using System.Reflection;

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
            var sourcedEvent = CreateSourcedEvent(_eventSourceId, _sequence++, eventData);
            _events.Add(sourcedEvent);
        }

        private static ISourcedEvent<IEventData> CreateSourcedEvent(Guid eventSourceId, long eventSequence, IEventData eventData)
        {
            var eventDataType = eventData.GetType();
            var sourcedEventType = typeof(SourcedEvent<>).MakeGenericType(eventDataType);

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var args = new object[] {eventSourceId, eventSequence, eventData};
            return (ISourcedEvent<IEventData>)Activator.CreateInstance(sourcedEventType, flags, null, args, CultureInfo.InvariantCulture);
        }

        public void Append(IEnumerable<IEventData> eventDatas)
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
