using System;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class EventDemultiplexerQueue
    {
        private readonly Guid _eventSourceId;
        private bool _blocked;
        private readonly Queue<SequencedEvent> _queue = new Queue<SequencedEvent>();

        public EventDemultiplexerQueue(Guid eventSourceId)
        {
            _eventSourceId = eventSourceId;
            _blocked = true;
        }

        public bool IsUnblocked
        {
            get { return !_blocked;}            
        }

        public void Unblock()
        {
            _blocked = false;
        }

        public bool IsEmpty
        {
            get { return _queue.Count == 0; }
        }

        public SequencedEvent Dequeue()
        {
            if (_blocked)
            {
                throw new InvalidOperationException("Can't Dequeue from blocked queue");
            }
            _blocked = true;            
            return _queue.Dequeue();
        }

        public bool Accept(SequencedEvent fetchedEvent)
        {
            if (fetchedEvent.Event.EventSourceId != _eventSourceId)
            {
                return false;
            }
            _queue.Enqueue(fetchedEvent);
            return true;
        }
    }
}