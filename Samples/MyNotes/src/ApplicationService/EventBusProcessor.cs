using System;
using Ncqrs.EventBus;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace ApplicationService
{
    public class EventBusProcessor : IElementProcessor
    {
        private readonly IEventBus _bus;

        public EventBusProcessor(IEventBus bus)
        {
            _bus = bus;
        }

        public void Process(IProcessingElement evnt)
        {
            var typedElement = (SourcedEventProcessingElement)evnt;
            Console.WriteLine("Processing event {0} (id {1})", typedElement.Event.EventSequence, typedElement.Event.EventIdentifier);
            _bus.Publish(typedElement.Event);
        }
    }
}