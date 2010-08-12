using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Ncqrs.EventBus
{
    public class EventDemultiplexer : IEventQueue
    {
        private int _pendingEventCount;
        private readonly Action<SequencedEvent> _enqueueToProcessingCallback;
        private readonly List<EventDemultiplexerQueue> _queues = new List<EventDemultiplexerQueue>();
        private readonly Dictionary<SequencedEvent, EventDemultiplexerQueue> _eventQueueMap = new Dictionary<SequencedEvent, EventDemultiplexerQueue>();

        public EventDemultiplexer(Action<SequencedEvent> enqueueToProcessingCallback)
        {
            _enqueueToProcessingCallback = enqueueToProcessingCallback;
        }

        public event EventHandler StateChanged;

        public int PendingEventCount
        {
            get { return _pendingEventCount; }
        }

        public void ProcessNext(SequencedEvent sequencedEvent)
        {
            var queue = FindQueueFor(sequencedEvent);
            if (queue != null)
            {
                queue.Enqueue(sequencedEvent);                
            }
            else
            {
                queue = CreateAndBlockQueueFor(sequencedEvent);
                AssociateEventAndQueue(sequencedEvent, queue);
                EnqueueToProcessing(sequencedEvent);
            }
            Interlocked.Increment(ref _pendingEventCount);
            OnStateChanged();
        }

        private void OnStateChanged()
        {
            if (StateChanged != null)
            {
                StateChanged(this, new EventArgs());
            }
        }

        private void AssociateEventAndQueue(SequencedEvent sequencedEvent, EventDemultiplexerQueue queue)
        {
            _eventQueueMap[sequencedEvent] = queue;
        }

        public void MarkAsProcessed(SequencedEvent sequencedEvent)
        {
            var queue = _eventQueueMap[sequencedEvent];
            queue.Unblock();
            Interlocked.Decrement(ref _pendingEventCount);
            OnStateChanged();
        }

        private void EnqueueToProcessing(SequencedEvent sequencedEvent)
        {
            _enqueueToProcessingCallback(sequencedEvent);
        }

        private EventDemultiplexerQueue CreateAndBlockQueueFor(SequencedEvent sequencedEvent)
        {
            var queue = new EventDemultiplexerQueue(sequencedEvent.Event.EventSourceId, _enqueueToProcessingCallback);
            _queues.Add(queue);
            return queue;
        }

        private EventDemultiplexerQueue FindQueueFor(SequencedEvent sequencedEvent)
        {
            return _queues.FirstOrDefault(x => x.Accepts(sequencedEvent));
        }
                      
    }
}