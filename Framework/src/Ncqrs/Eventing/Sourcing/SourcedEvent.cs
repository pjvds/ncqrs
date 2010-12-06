using System;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Eventing.Sourcing
{
    [Serializable]
    public abstract class SourcedEvent : Event, ISourcedEvent
    {
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
            EventSourceId = UndefinedValues.UndefinedEventSourceId;
            EventSequence = UndefinedValues.UndefinedEventSequence;
        }


        public SourcedEvent(Guid eventIdentifier, Guid eventSourceId, long eventSequence, DateTime eventTimeStamp)
            : base(eventIdentifier, eventTimeStamp)
        {
            EventSourceId = eventSourceId;
            EventSequence = eventSequence;
        }


        public virtual void InitializeFrom(StoredEvent stored)
        {
            // TODO: this does not update the event version information. That information isn't even exposed here.
            EventIdentifier = stored.EventIdentifier;
            EventSourceId = stored.EventSourceId;
            EventSequence = stored.EventSequence;
            EventTimeStamp = stored.EventTimeStamp;
        }


        public void ClaimEvent(Guid eventSourceId, long sequence)
        {
            if (this.EventSourceId != UndefinedValues.UndefinedEventSourceId) throw new InvalidOperationException(string.Format("The event is already owned by event source with id {0}.", this.EventSourceId));

            EventSourceId = eventSourceId;
            EventSequence = sequence;
        }
    }
}
