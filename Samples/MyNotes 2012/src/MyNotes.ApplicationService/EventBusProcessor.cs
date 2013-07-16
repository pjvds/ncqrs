using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ncqrs.EventBus;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace MyNotes.ApplicationService
{
    internal class EventBusProcessor : IElementProcessor
    {
        private readonly IEventBus eventBus;

        public EventBusProcessor(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public void Process(IProcessingElement e)
        {
            var typedElement = (SourcedEventProcessingElement)e;

            Console.WriteLine("Processing event {0} (id {1})", typedElement.Event.EventSequence, typedElement.Event.EventIdentifier);
            this.eventBus.Publish(typedElement.Event);
        }
    }
}
