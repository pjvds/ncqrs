using System;

namespace Ncqrs.Domain.Mapping
{
    /// <summary>
    /// Occurs when no handler was found for for a <see cref="IEvent"/>.
    /// </summary>
    public class NoEventHandlerFoundException : Exception
    {
        /// <summary>
        /// Gets the event.
        /// </summary>
        public IEvent Event
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventHandlerFoundException"/> class.
        /// </summary>
        /// <param name="evnt">The event.</param>
        public NoEventHandlerFoundException(IEvent evnt)
            : this(evnt, String.Format("No handler found for event {0}.", evnt))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventHandlerFoundException"/> class.
        /// </summary>
        /// <param name="evnt">The evnt.</param>
        /// <param name="message">The message.</param>
        public NoEventHandlerFoundException(IEvent evnt, string message)
            : this(evnt, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventHandlerFoundException"/> class.
        /// </summary>
        /// <param name="evnt">The evnt.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NoEventHandlerFoundException(IEvent evnt, string message, Exception innerException)
            : base(message, innerException)
        {
            Event = evnt;
        }
    }
}
