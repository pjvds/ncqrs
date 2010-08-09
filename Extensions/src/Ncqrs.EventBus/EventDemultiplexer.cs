using System;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class EventDemultiplexerQueue
    {
        
    }

    public class EventDemultiplexer
    {
        private readonly IEventStore _eventStore;
        private readonly List<EventDemultiplexerQueue> _queues = new List<EventDemultiplexerQueue>();

        public EventDemultiplexer(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public SequencedEvent GetNext()
        {
            return _eventStore.GetNext();
        }
    }
}