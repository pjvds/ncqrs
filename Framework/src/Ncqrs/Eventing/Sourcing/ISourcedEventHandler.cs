using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing.Sourcing
{
    /// <summary>
    /// An event handler that handles the domain events.
    /// </summary>
    [ContractClass(typeof(IEventSourcedHandlerContracts))]
    public interface ISourcedEventHandler
    {
        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="sourcedEvent">The event to handle.</param>
        /// <returns><c>true</c> when the event was handled; otherwise, <c>false</c>.
        /// <remarks><c>false</c> does not mean that the handling failed, but that the 
        /// handler was not interested in handling this event.</remarks></returns>
        Boolean HandleEvent(object sourcedEvent);
    }

    [ContractClassFor(typeof(ISourcedEventHandler))]
    internal abstract class IEventSourcedHandlerContracts : ISourcedEventHandler
    {
        public bool HandleEvent(object sourcedEvent)
        {
            Contract.Requires<ArgumentNullException>(sourcedEvent != null, "The sourcedEvent cannot be null.");

            return default(bool);
        }
    }
}