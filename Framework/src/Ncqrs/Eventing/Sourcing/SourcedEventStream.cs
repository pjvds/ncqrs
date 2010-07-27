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
        private readonly IList<SourcedEvent> _events = new List<SourcedEvent>();

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

        /// <summary>
        /// Gets the last sequence in this stream.
        /// </summary>
        /// <value>The last sequence.</value>
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
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                _sequenceOffset = value;
            }
        }


        /// <summary>
        /// Gets the number of events in this stream.
        /// </summary>
        /// <value>The number of events in this stream. This value is zero when <see cref="IsEmpty"/> is <c>true</c>.</value>
        public int Count
        {
            get
            {
                return _events.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
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

        public SourcedEventStream(Guid eventSourceId) : this(eventSourceId, sequenceOffset: 0)
        {
        }

        public SourcedEventStream(Guid eventSourceId, long sequenceOffset)
        {
            Contract.Requires(sequenceOffset >= 0);

            _eventSourceId = eventSourceId;
            _sequenceOffset = sequenceOffset;
        }

        [ContractInvariantMethod]
        private void ContractInvariants()
        {
            Contract.Invariant(_sequenceOffset >= 0);
            Contract.Invariant(Contract.ForAll(_events, (sourcedEvent) => sourcedEvent.EventSourceId == _eventSourceId));
            Contract.Invariant(Contract.ForAll(_events, (sourcedEvent) => sourcedEvent.EventSequence == (_sequenceOffset + _events.IndexOf(sourcedEvent) + 1)));
        }

        /// <summary>
        /// Appends the specified sourced event to this steam.
        /// </summary>
        /// <exception cref="ArgumentNullException">Occurs when <paramref name="sourcedEvent"/> is null.</exception>
        /// <exception cref="ArgumentException">Occurs when <paramref name="sourcedEvent.EventSourceId"/> is not owned set to the <see cref="EventSourceId"/> property of this stream.</exception>
        /// <exception cref="ArgumentException">Occurs when <paramref name="sourcedEvent.Sequence"/> is not set to <see cref="LastSequence"/><c>+1</c>.</exception>
        /// <param name="sourcedEvent">The sourced event.</param>
        public void Append(SourcedEvent sourcedEvent)
        {
            ValidateSourcedEvent(sourcedEvent);

            _events.Add(sourcedEvent);
        }

        public void Append(IEnumerable<SourcedEvent> events)
        {
            if (events == null) throw new ArgumentNullException("events");

            foreach (var evnt in events)
            {
                Append(evnt);
            }
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

                throw new ArgumentException(msg, "sourcedEvent");
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