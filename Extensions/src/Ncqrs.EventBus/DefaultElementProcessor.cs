using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.EventBus
{
    
    public class DefaultElementProcessor : IElementProcessor
    {
        private readonly InProcessEventBus _internalBus;
        
        public DefaultElementProcessor(bool useTransactionScope)
        {
            _internalBus = new InProcessEventBus(useTransactionScope);
        }

        public void Process(IProcessingElement processingElement)
        {
            var typedElement = (SourcedEventProcessingElement) processingElement;
            _internalBus.Publish(typedElement.Event);
        }

        public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler)
        {
            _internalBus.RegisterHandler(handler);
        }
    }
}