using System;

namespace Ncqrs.EventBus
{
    public class EventDemultiplexer
    {
        private readonly IEventStore _eventStore;

        public EventDemultiplexer(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public SequencedEvent GetNext()
        {
            return null;
        }
    }
}