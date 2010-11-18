using System;

namespace Ncqrs.EventBus
{
    public class EventProcessedEventArgs : EventArgs
    {
        private readonly SequencedEvent _event;

        public EventProcessedEventArgs(SequencedEvent @event)
        {
            _event = @event;
        }

        public SequencedEvent Event
        {
            get { return _event; }
        }
    }
}