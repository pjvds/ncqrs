using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public class SequencedEvent
    {
        private readonly int _sequence;
        private readonly SourcedEvent _event;

        public SequencedEvent(int sequence, SourcedEvent evnt)
        {
            _sequence = sequence;
            _event = evnt;
        }

        public SourcedEvent Event
        {
            get { return _event; }
        }

        public int Sequence
        {
            get { return _sequence; }
        }
    }
}