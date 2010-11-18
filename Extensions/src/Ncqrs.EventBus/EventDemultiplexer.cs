using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class EventDemultiplexer : IEventQueue
    {
        private readonly List<EventDemultiplexerQueue> _queues = new List<EventDemultiplexerQueue>();
        private readonly Dictionary<Guid, EventDemultiplexerQueue> _eventQueueMap = new Dictionary<Guid, EventDemultiplexerQueue>();
        
        public event EventHandler<EventDemultiplexedEventArgs> EventDemultiplexed;

        private void OnEventDemultiplexed(EventDemultiplexedEventArgs e)
        {
            var handler = EventDemultiplexed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Demultiplex(SequencedEvent sequencedEvent)
        {
            if (IsDuplicate(sequencedEvent))
            {
                Debug.WriteLine("Ignoring duplicate event {0}.", sequencedEvent.Event.EventIdentifier);
                return;
            }
            var queue = FindQueueFor(sequencedEvent);
            if (queue != null)
            {
                AssociateEventAndQueue(sequencedEvent, queue);
                queue.Enqueue(sequencedEvent);
            }
            else
            {
                queue = CreateAndBlockQueueFor(sequencedEvent);
                AssociateEventAndQueue(sequencedEvent, queue);
                EnqueueToProcessing(sequencedEvent);
            }
        }

        private bool IsDuplicate(SequencedEvent sequencedEvent)
        {
            return _eventQueueMap.ContainsKey(sequencedEvent.Event.EventIdentifier);
        }

        private void AssociateEventAndQueue(SequencedEvent sequencedEvent, EventDemultiplexerQueue queue)
        {
            _eventQueueMap.Add(sequencedEvent.Event.EventIdentifier, queue);
        }

        public void MarkAsProcessed(SequencedEvent sequencedEvent)
        {
            var eventId = sequencedEvent.Event.EventIdentifier;
            var queue = _eventQueueMap[eventId];            
            _eventQueueMap.Remove(eventId);
            if (!queue.Unblock())
            {
                _queues.Remove(queue);
            }            
        }

        private void EnqueueToProcessing(SequencedEvent sequencedEvent)
        {
            OnEventDemultiplexed(new EventDemultiplexedEventArgs(sequencedEvent));
        }

        private EventDemultiplexerQueue CreateAndBlockQueueFor(SequencedEvent sequencedEvent)
        {
            var queue = new EventDemultiplexerQueue(sequencedEvent.Event.EventSourceId, EnqueueToProcessing);
            _queues.Add(queue);
            return queue;
        }

        private EventDemultiplexerQueue FindQueueFor(SequencedEvent sequencedEvent)
        {
            return _queues.FirstOrDefault(x => x.Accepts(sequencedEvent));
        }

    }
}