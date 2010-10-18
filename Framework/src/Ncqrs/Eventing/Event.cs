using System;

namespace Ncqrs.Eventing
{
    /// <summary>
    /// The base for all event messages. All sourced events should subclass from <see cref="Ncqrs.Eventing.Sourcing.ISourcedEvent"/>.
    /// </summary>
    [Serializable]
    public abstract class Event : IEvent
    {
        private static Version DefaultVersion = new Version(1,0);

        /// <summary>
        /// Gets the unique identifier for this event.
        /// </summary>
        public Guid EventIdentifier { get; internal protected set; }

        /// <summary>
        /// Gets the time stamp for this event.
        /// </summary>
        /// <value>a <see cref="DateTime"/> UTC value that represents the point
        /// in time where this event occurred.</value>
        public DateTime EventTimeStamp { get; internal protected set; }

        /// <summary>
        /// Gets the event version.
        /// </summary>
        /// <value>The event version.</value>
        public Version EventVersion { get; internal protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        public Event()
        {
            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            var clock = NcqrsEnvironment.Get<IClock>();

            EventIdentifier = idGenerator.GenerateNewId();
            EventTimeStamp = clock.UtcNow();
            EventVersion = DefaultVersion;
        }

        public Event(Guid eventIdentifier, DateTime eventTimeStamp)
            : this(eventIdentifier, eventTimeStamp, DefaultVersion)
        {
        }

        public Event(Guid eventIdentifier, DateTime eventTimeStamp, Version eventVersion)
        {
            EventIdentifier = eventIdentifier;
            EventTimeStamp = eventTimeStamp;
            EventVersion = eventVersion;
        }
    }
}