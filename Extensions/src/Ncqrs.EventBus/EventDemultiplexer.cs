using System;
using System.Linq;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
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
            var queue = FindFirstUnblockedQueue();
            if (queue != null)
            {
                return DequeueFromExistingQueue(queue);
            }
            var fetchedEvent = FetchForNewQueue();
            CreateQueueFor(fetchedEvent.Event.EventSourceId);
            return fetchedEvent;
        }

        private SequencedEvent FetchForNewQueue()
        {
            bool enqueued;
            SequencedEvent fetchedEvent;
            do 
            {
                fetchedEvent = _eventStore.GetNext();
                enqueued = PlaceEventInQueue(fetchedEvent);
            }
            while (enqueued);
            return fetchedEvent;
        }

        private SequencedEvent DequeueFromExistingQueue(EventDemultiplexerQueue queue)
        {
            var result = queue.Dequeue();
            if (queue.IsEmpty)
            {
                _queues.Remove(queue);
            }
            return result;
        }

        private bool PlaceEventInQueue(SequencedEvent fetchedEvent)
        {
            return _queues.FirstOrDefault(x => x.Accept(fetchedEvent)) != null;
        }

        private void CreateQueueFor(Guid eventSourceId)
        {
            _queues.Add(new EventDemultiplexerQueue(eventSourceId));
        }

        private EventDemultiplexerQueue FindFirstUnblockedQueue()
        {
            return _queues.FirstOrDefault(x => x.IsUnblocked);
        }
    }
}