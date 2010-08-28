using System;

namespace Ncqrs.Eventing.Sourcing
{
    public class SourcedEventHandlerPredicate<TSourcedEvent> : TypeThresholdedActionBasedDomainEventHandler
        where TSourcedEvent : SourcedEvent
    {
        private readonly Predicate<TSourcedEvent> _predicate;
        private readonly ISourcedEventHandler _internalHandler;

        public SourcedEventHandlerPredicate(Predicate<TSourcedEvent> predicate, ISourcedEventHandler internalHandler) : base((e)=>internalHandler.HandleEvent(e), typeof(TSourcedEvent), false)
        {
            _predicate = predicate;
            _internalHandler = internalHandler;
        }
    }

    public class TypeAndCallbackThresholdedActionBasedDomainEventHandler<TEvent> : TypeAndCallbackThresholdedActionBasedDomainEventHandler
        where TEvent : SourcedEvent
    {
        private readonly TypeAndCallbackThresholdedActionBasedDomainEventHandler _internalHandle;

        public TypeAndCallbackThresholdedActionBasedDomainEventHandler(Action<TEvent> handler, Func<TEvent, bool> filterCallback) : base((e)=>handler((TEvent)e), (e)=>filterCallback((TEvent)e), typeof(TEvent))
        {
            
        }
    }
}