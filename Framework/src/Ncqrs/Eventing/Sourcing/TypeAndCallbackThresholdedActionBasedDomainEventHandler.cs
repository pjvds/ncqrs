using System;

namespace Ncqrs.Eventing.Sourcing
{
    ///<summary>
    /// An event handler that uses a specified action as handler, but only calls this when the event
    /// is accepted by provided callback method and is of a certain type, or is inherited from it.
    ///</summary>
    public class TypeAndCallbackThresholdedActionBasedDomainEventHandler : ISourcedEventHandler
    {
        private readonly Func<SourcedEvent, bool> _filterCallback;
        private readonly TypeThresholdedActionBasedDomainEventHandler _internalHandler;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TypeAndCallbackThresholdedActionBasedDomainEventHandler" /> class.
        /// </summary>
        /// <param name = "handler">The handler that will be called to handle a event when the threshold did not hold the event.</param>
        /// <param name="filterCallback">A filter function.</param>
        /// <param name = "eventTypeThreshold">The event type that should be used as threshold.</param>
        /// <param name = "exact">if set to <c>true</c> the threshold will hold all types that are not the same type; otherwise it hold 
        /// all types that are not inhered from the event type threshold or implement the interface that is specified by the threshold type.</param>
        public TypeAndCallbackThresholdedActionBasedDomainEventHandler(Action<SourcedEvent> handler, Func<SourcedEvent, bool> filterCallback, Type eventTypeThreshold,
                                                                       Boolean exact = false)
        {
            _internalHandler = new TypeThresholdedActionBasedDomainEventHandler(handler, eventTypeThreshold, exact);
            _filterCallback = filterCallback;
        }

        public bool HandleEvent(SourcedEvent sourcedEvent)
        {
            return _filterCallback(sourcedEvent) && _internalHandler.HandleEvent(sourcedEvent);
        }
    }
}