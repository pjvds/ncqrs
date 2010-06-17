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

        /// <summary>
        /// Gets or sets the id of the <see cref="IEventSource"/> that owns the events.
        /// This property can only be set when the event is <see cref="IsEmpty"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Occurs when this property is set while <see cref="IsEmpty"/> is true.</exception>
        /// <value>
        /// A <see cref="Guid"/> that contains the id of the <see cref="IEventSource"/> that
        /// owns this event stream.
        /// </value>
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

        public long LastSequence
        {
            get
            {
                return SequenceOffset + Count;
            }
        }

        /// <summary>
        /// Gets or sets the offset of the event stream. This has influence on the sequence number
        /// </summary>
        /// <exception cref="InvalidOperationException">Occurs when this property is set while <see cref="IsEmpty"/> is true.</exception>
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
            Contract.Invariant(Contract.ForAll(_events, (sourcedEvent) => sourcedEvent.EventSequence == (_sequenceOffset + _events.IndexOf(sourcedEvent) + 1)));
        }

        public void Append(SourcedEvent sourcedEvent)
        {
            ValidateSourcedEvent(sourcedEvent);

            _events.Add(sourcedEvent);
        }

        private void ValidateSourcedEvent(SourcedEvent sourcedEvent)
        {
            if (sourcedEvent == null) throw new ArgumentNullException("sourcedEvent");

            if (sourcedEvent.EventSourceId != EventSourceId)
            {
                var msg = String.Format("Cannot apply event that is associated to another "+
                                        "event source. The event belongs to event "+
                                        "source with id {0} where {1} was expected.",
                                        sourcedEvent.EventSourceId, EventSourceId);
                
                throw new ArgumentException(msg, "sourcedEvent");
            }

            long requiredSequence = LastSequence + 1;

            if (sourcedEvent.EventSequence != requiredSequence)
            {

                var msg = String.Format("Cannot apply event with incorrect sequence number. "+
                                        "The sequence number of the event is {0} while {1} was required.",
                                        sourcedEvent.EventSequence, requiredSequence);

                throw new ArgumentException("Cannot apply event with incorrect sequence number. The sequence number of the event is {0} while {1} was required.");
            }
        }

        public void Append(IEnumerable<SourcedEvent> eventDatas)
        {
            foreach (var data in eventDatas)
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
