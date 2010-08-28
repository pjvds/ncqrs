using System;

namespace Ncqrs.Eventing.Sourcing
{
    public class SourcedEventHandlerPredicate<TSourcedEvent> : TypeThresholdedActionBasedDomainEventHandler<TSourcedEvent>
        where TSourcedEvent : SourcedEvent
    {
        private readonly Predicate<TSourcedEvent> _predicate;
        private readonly ISourcedEventHandler _internalHandler;

        public SourcedEventHandlerPredicate(Predicate<TSourcedEvent> predicate, ISourcedEventHandler internalHandler)
            : base((e) => internalHandler.HandleEvent(e), false)
        {
            _predicate = predicate;
            _internalHandler = internalHandler;
        }
    }
}