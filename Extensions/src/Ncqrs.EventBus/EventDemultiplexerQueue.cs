using System;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class EventDemultiplexerQueue
    {
        private readonly Guid _eventSourceId;
        private readonly Action<SequencedEvent> _enqueueToProcessingCallback;
        private readonly Queue<SequencedEvent> _queue = new Queue<SequencedEvent>();

        public EventDemultiplexerQueue(Guid eventSourceId, Action<SequencedEvent> enqueueToProcessingCallback)
        {
            _eventSourceId = eventSourceId;
            _enqueueToProcessingCallback = enqueueToProcessingCallback;
        }
        
        public void Unblock()
        {
            if (_queue.Count > 0)
            {
                _enqueueToProcessingCallback(_queue.Dequeue());
            }
        }
                
        public bool Accepts(SequencedEvent fetchedEvent)
        {
            return fetchedEvent.Event.EventSourceId == _eventSourceId;
        }

        public void Enqueue(SequencedEvent sequencedEvent)
        {
            _queue.Enqueue(sequencedEvent);
        }
    }
}