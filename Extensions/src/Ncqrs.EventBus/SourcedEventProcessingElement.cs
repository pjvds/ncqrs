using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public class SourcedEventProcessingElement : IProcessingElement
    {
        private readonly IPublishableEvent _event;

        public SourcedEventProcessingElement(IPublishableEvent evnt)
        {
            _event = evnt;
        }
        
        public SourcedEventProcessingElement(int sequenceNumber, IPublishableEvent @event)
        {
            SequenceNumber = sequenceNumber;
            _event = @event;
        }

        public int SequenceNumber { get; set; }

        public string UniqueId
        {
            get
            {
                return Event.EventIdentifier.ToString();
            }
        }
        public object GroupingKey
        {
            get { return Event.EventSourceId; }
        }

        public IPublishableEvent Event
        {
            get { return _event; }
        }
    }
}