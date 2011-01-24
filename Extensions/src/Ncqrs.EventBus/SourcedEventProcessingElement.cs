using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public class SourcedEventProcessingElement : IProcessingElement
    {
        private readonly SourcedEvent _event;

        public SourcedEventProcessingElement(SourcedEvent @event)
        {
            _event = @event;
        }

        public SourcedEventProcessingElement(int sequenceNumber, SourcedEvent @event)
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

        public SourcedEvent Event
        {
            get { return _event; }
        }
    }
}