using System;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    [Serializable]
    public abstract class DomainEvent : ISourcedEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event.
        /// </summary>
        /// <value></value>
        public Guid EventIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the id of the event source that owns this event.
        /// </summary>
        /// <value>The id of the event source that owns this event.</value>
        public Guid EventSourceId
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the event sequence.
        /// </summary>
        /// <value>A number that represents the number of this event in the sequence.</value>
        public long EventSequence
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the time stamp for this event.
        /// </summary>
        /// <value>
        /// a <see cref="DateTime"/> UTC value that represents the point
        /// in time where this event occurred.
        /// </value>
        public DateTime EventTimeStamp
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        /// <remarks>This initializes the <see cref="EventIdentifier"/> with the
        /// value from the <see cref="IUniqueIdentifierGenerator"/> and the 
        /// <see cref="EventTimeStamp"/> with the value from the 
        /// <see cref="IClock"/> that is gotten from the 
        /// <see cref="NcqrsEnvironment"/>.</remarks>
        protected DomainEvent()
        {
            EventSourceId = Guid.Empty;

            var clock = NcqrsEnvironment.Get<IClock>();
            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();

            EventTimeStamp = clock.UtcNow();
            EventIdentifier = idGenerator.GenerateNewId();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        /// <param name="eventIdentifier">The event identifier.</param>
        /// <param name="eventSourceId">The id of the event source that caused this event.</param>
        /// <param name="eventSequence">The event sequence.</param>
        /// <param name="eventTimeStamp">The event time stamp.</param>
        protected DomainEvent(Guid eventIdentifier, Guid eventSourceId, long eventSequence, DateTime eventTimeStamp)
        {
            EventIdentifier = eventIdentifier;
            EventSourceId = eventSourceId;
            EventSequence = eventSequence;
            EventTimeStamp = eventTimeStamp;
        }
    }
}