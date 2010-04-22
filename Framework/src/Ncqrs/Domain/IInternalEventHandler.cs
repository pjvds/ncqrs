using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    /// <summary>
    /// An event handler that handles the event internally. Where internally means <i>inside the domain</i>.
    /// </summary>
    [ContractClass(typeof(IInternalEventHandlerContracts))]
    public interface IInternalEventHandler
    {
        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="evnt">The event to handle.</param>
        /// <returns><c>true</c> when the event was handled; otherwise, <c>false</c>.
        /// <remarks><c>false</c> does not mean that the handling failed, but that the 
        /// handler was not interested in handling this event.</remarks></returns>
        Boolean HandleEvent(IEvent evnt);
    }

    [ContractClassFor(typeof(IInternalEventHandler))]
    public class IInternalEventHandlerContracts : IInternalEventHandler
    {
        public bool HandleEvent(IEvent evnt)
        {
            Contract.Requires<ArgumentNullException>(evnt != null, "The evnt cannot be null.");

            return default(bool);
        }
    }
}
