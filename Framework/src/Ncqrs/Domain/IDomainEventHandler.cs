using System;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    /// <summary>
    /// An event handler that handles the domain events.
    /// </summary>
    [ContractClass(typeof(DomainEventHandlerContracts))]
    public interface IDomainEventHandler
    {
        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="sourcedEvent">The event to handle.</param>
        /// <returns><c>true</c> when the event was handled; otherwise, <c>false</c>.
        /// <remarks><c>false</c> does not mean that the handling failed, but that the 
        /// handler was not interested in handling this event.</remarks></returns>
        Boolean HandleEvent(ISourcedEvent sourcedEvent);
    }

    [ContractClassFor(typeof(IDomainEventHandler))]
    public class DomainEventHandlerContracts : IDomainEventHandler
    {
        public bool HandleEvent(ISourcedEvent sourcedEvent)
        {
            Contract.Requires<ArgumentNullException>(sourcedEvent != null, "The sourcedEvent cannot be null.");

            return default(bool);
        }
    }
}
