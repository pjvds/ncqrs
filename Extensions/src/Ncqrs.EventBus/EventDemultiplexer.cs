using System;
using System.Linq;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class EventDemultiplexer : IEventQueue
    {
        private readonly Action<SequencedEvent> _enqueueToProcessingCallback;
        private readonly IPipelineStateMonitor _stateMonitor;
        private readonly List<EventDemultiplexerQueue> _queues = new List<EventDemultiplexerQueue>();
        private readonly Dictionary<SequencedEvent, EventDemultiplexerQueue> _eventQueueMap = new Dictionary<SequencedEvent, EventDemultiplexerQueue>();

        public EventDemultiplexer(Action<SequencedEvent> enqueueToProcessingCallback, IPipelineStateMonitor stateMonitor)
        {
            _enqueueToProcessingCallback = enqueueToProcessingCallback;
            _stateMonitor = stateMonitor;
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
            _stateMonitor.OnDemultiplexed();
        }

        private void AssociateEventAndQueue(SequencedEvent sequencedEvent, EventDemultiplexerQueue queue)
        {
            _eventQueueMap[sequencedEvent] = queue;
        }

        public void MarkAsProcessed(SequencedEvent sequencedEvent)
        {
            var queue = _eventQueueMap[sequencedEvent];
            queue.Unblock();
            _stateMonitor.OnProcessed();
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