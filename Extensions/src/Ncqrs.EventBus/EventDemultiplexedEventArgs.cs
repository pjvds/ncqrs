using System;

namespace Ncqrs.EventBus
{
    public class EventDemultiplexedEventArgs : EventArgs
    {
        private readonly SequencedEvent _event;

        public EventDemultiplexedEventArgs(SequencedEvent @event)
        {
            _event = @event;
        }

        public SequencedEvent Event
        {
            get { return _event; }
        }
    }
}