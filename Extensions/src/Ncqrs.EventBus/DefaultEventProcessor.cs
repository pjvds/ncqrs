using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public class DefaultEventProcessor : IEventProcessor
    {
        private readonly InProcessEventBus _internalBus;
        
        public DefaultEventProcessor(bool useTransactionScope)
        {
            _internalBus = new InProcessEventBus(useTransactionScope);
        }

        public void Process(SourcedEvent evnt)
        {
            _internalBus.Publish(evnt);
        }

        public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            _internalBus.RegisterHandler(handler);
        }
    }
}