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
        /// Gets the aggregate root id.
        /// </summary>
        /// <value>The aggregate root id of the one that owns this event.</value>
        public Guid AggregateRootId
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

        Guid ISourcedEvent.EventSourceId
        {
            get
            {
                return AggregateRootId;
            }
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
            AggregateRootId = Guid.Empty;

            var clock = NcqrsEnvironment.Get<IClock>();
            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();

            EventTimeStamp = clock.UtcNow();
            EventIdentifier = idGenerator.GenerateNewId();
        }
    }
}