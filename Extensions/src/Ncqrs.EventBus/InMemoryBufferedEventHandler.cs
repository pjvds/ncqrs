using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public class InMemoryBufferedEventHandler : IEventHandler<SourcedEvent>
    {
        private readonly InMemoryBufferedBrowsableElementStore _buffer;

        public InMemoryBufferedEventHandler(InMemoryBufferedBrowsableElementStore buffer)
        {
            _buffer = buffer;
        }

        public void Handle(SourcedEvent evnt)
        {
            _buffer.PushElement(new SourcedEventProcessingElement(evnt));
        }
    }
}