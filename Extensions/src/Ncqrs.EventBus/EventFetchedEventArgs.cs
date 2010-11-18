using System;

namespace Ncqrs.EventBus
{
    public class EventFetchedEventArgs : EventArgs
    {
        private readonly SequencedEvent _event;

        public EventFetchedEventArgs(SequencedEvent @event)
        {
            _event = @event;
        }

        public SequencedEvent Event
        {
            get { return _event; }
        }
    }
}