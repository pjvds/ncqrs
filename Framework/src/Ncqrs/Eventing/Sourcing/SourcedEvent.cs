using System;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Eventing.Sourcing
{
    [Serializable]
    public abstract class SourcedEvent : Event, ISourcedEvent
    {
        public static Guid _UndefinedEventSourceId = Guid.Empty;
        public const int _UndefinedEventSequence = -1;

        public Guid UndefinedEventSourceId
        {
            get
            {
                return _UndefinedEventSourceId;
            }
        }

        public int UndefinedEventSequence
        {
            get
            {
                return _UndefinedEventSequence;
            }
        }

        /// <summary>
        /// Gets the id of the event source that caused the event.
        /// </summary>
        /// <value>The id of the event source that caused the event.</value>
        public Guid EventSourceId
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the event sequence number.
        /// </summary>
        /// <remarks>
        /// An sequence of events always starts with <c>1</c>. So the first event in a sequence has the <see cref="EventSequence"/> value of <c>1</c>.
        /// </remarks>
        /// <value>A number that represents the order of where this events occurred in the sequence.</value>
        public long EventSequence
        {
            get;
            internal set;
        }

        public void ApplyEventSourceIdAndSequence(Guid SourceId, long sequence)
        {
            this.EventSourceId = SourceId;
            this.EventSequence = sequence;
        }

        public void ApplyEventInformation(Guid EventIdentifier, DateTime EventTimeStamp, Version EventVersion, Guid EventSourceId, long EventSequence)
        {
            this.EventIdentifier = EventIdentifier;
            this.EventTimeStamp = EventTimeStamp;
            this.EventVersion = EventVersion;
            this.EventSourceId = EventSourceId;
            this.EventSequence = EventSequence;
        }

        public SourcedEvent()
        {
            EventSourceId = UndefinedEventSourceId;
            EventSequence = UndefinedEventSequence;
        }


        public SourcedEvent(Guid eventIdentifier, Guid eventSourceId, long eventSequence, DateTime eventTimeStamp)
            : base(eventIdentifier, eventTimeStamp)
        {
            EventSourceId = eventSourceId;
            EventSequence = eventSequence;
        }


        public virtual void InitializeFrom(StoredEvent stored)
        {
            EventIdentifier = stored.EventIdentifier;
            EventSourceId = stored.EventSourceId;
            EventSequence = stored.EventSequence;
            EventTimeStamp = stored.EventTimeStamp;
        }
    }
}
