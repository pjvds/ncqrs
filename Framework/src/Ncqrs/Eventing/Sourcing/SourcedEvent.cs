using System;

namespace Ncqrs.Eventing.Sourcing
{
    [Serializable]
    public abstract class SourcedEvent : Event, ISourcedEvent
    {
        public static Guid UndefinedEventSourceId = Guid.Empty;
        public const int UndefinedEventSequence = -1;

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
    }
}