using System.Collections.Generic;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    /// <summary>
    /// A bus that can publish events to handlers.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes the event to the handlers.
        /// </summary>
        /// <param name="eventMessage">The message to publish.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>message</i> was null.</exception>
        /// <exception cref="NoHandlerRegisteredForMessageException">Thrown when no handler was found for the specified message.</exception>
        void Publish(IEvent eventMessage);

        /// <summary>
        /// Publishes the messages to the handlers.
        /// </summary>
        /// <param name="eventMessage">The messages to publish.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>messages</i> was null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a instance in <i>messages</i> was null.</exception>
        /// <exception cref="NoHandlerRegisteredForMessageException">Thrown when no handler was found for one of the specified messages.</exception>
        void Publish(IEnumerable<IEvent> eventMessages);        
    }
}