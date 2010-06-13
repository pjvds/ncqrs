using System;

namespace Ncqrs.Eventing.Sourcing
{
    [Serializable]
    public abstract class SourcedEventBase : EventBase, ISourcedEvent
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
        /// An sequence of events always starts with <c>0</c>. So the first event in a sequence has the <see cref="EventSequence"/> value of <c>0</c>.
        /// </remarks>
        /// <value>A number that represents the order of where this events occurred in the sequence.</value>
        public long EventSequence
        {
            get;
            internal set;
        }

        internal SourcedEventBase(Guid eventSourceId, long eventSequence)
        {
            EventSourceId = eventSourceId;
            EventSequence = eventSequence;
        }
    }
}