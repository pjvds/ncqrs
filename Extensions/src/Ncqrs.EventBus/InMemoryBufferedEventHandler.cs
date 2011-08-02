using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public class InMemoryBufferedEventHandler : IEventHandler<object>
    {
        private readonly InMemoryBufferedBrowsableElementStore _buffer;

        public InMemoryBufferedEventHandler(InMemoryBufferedBrowsableElementStore buffer)
        {
            _buffer = buffer;
        }

        public void Handle(IPublishedEvent<object> evnt)
        {
            _buffer.PushElement(new SourcedEventProcessingElement(evnt));
        }
    }
}