using System;

namespace Ncqrs.Eventing
{
    /// <summary>
    /// The base for all event messages.
    /// </summary>
    [Serializable]
    public abstract class EventBase : IEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event.
        /// </summary>
        public Guid EventIdentifier { get; private set; }

        /// <summary>
        /// Gets the time stamp for this event.
        /// </summary>
        /// <value>a <see cref="DateTime"/> UTC value that represents the point
        /// in time where this event occurred.</value>
        public DateTime EventTimeStamp { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBase"/> class.
        /// </summary>
        protected EventBase()
        {
            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            var clock = NcqrsEnvironment.Get<IClock>();

            EventIdentifier = idGenerator.GenerateNewId();
            EventTimeStamp = clock.UtcNow();
        }
    }
}
