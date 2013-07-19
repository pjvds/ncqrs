using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class Demultiplexer// : IEventQueue
    {
        private readonly List<DemultiplexerQueue> _queues = new List<DemultiplexerQueue>();
        private readonly Dictionary<string, DemultiplexerQueue> _queueMap = new Dictionary<string, DemultiplexerQueue>();
        
        public event EventHandler<ElementDemultiplexedEventArgs> EventDemultiplexed;

        private void OnEventDemultiplexed(ElementDemultiplexedEventArgs e)
        {
            var handler = EventDemultiplexed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Demultiplex(IProcessingElement sequencedEvent)
        {
            if (IsDuplicate(sequencedEvent))
            {
                Debug.WriteLine("Ignoring duplicate element {0}.", sequencedEvent.UniqueId);
                return;
            }
            var queue = FindQueueFor(sequencedEvent);
            if (queue != null)
            {
                AssociateElementAndQueue(sequencedEvent, queue);
                queue.Enqueue(sequencedEvent);
                EnqueueToProcessing(sequencedEvent);
            }
            else
            {
                queue = CreateAndBlockQueueFor(sequencedEvent);
                AssociateElementAndQueue(sequencedEvent, queue);
                EnqueueToProcessing(sequencedEvent);
            }
        }

        private bool IsDuplicate(IProcessingElement processingElement)
        {
            return _queueMap.ContainsKey(processingElement.UniqueId);
        }

        private void AssociateElementAndQueue(IProcessingElement processingElement, DemultiplexerQueue queue)
        {
            _queueMap.Add(processingElement.UniqueId, queue);
        }

        public void MarkAsProcessed(IProcessingElement processingElement)
        {
            var queue = _queueMap[processingElement.UniqueId];            
            _queueMap.Remove(processingElement.UniqueId);
            if (queue.IsEmpty())
            {
                _queues.Remove(queue);
            }
        }

        private void EnqueueToProcessing(IProcessingElement processingElement)
        {
            OnEventDemultiplexed(new ElementDemultiplexedEventArgs(processingElement));
        }

        private DemultiplexerQueue CreateAndBlockQueueFor(IProcessingElement processingElement)
        {
            var queue = new DemultiplexerQueue(processingElement.GroupingKey);
            _queues.Add(queue);
            return queue;
        }

        private DemultiplexerQueue FindQueueFor(IProcessingElement processingElement)
        {
            return _queues.FirstOrDefault(x => x.Accepts(processingElement));
        }

    }
}