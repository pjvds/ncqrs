using System;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class DemultiplexerQueue
    {
        private readonly object _groupingKey;
        private readonly Action<IProcessingElement> _enqueueToProcessingCallback;
        private readonly Queue<IProcessingElement> _queue = new Queue<IProcessingElement>();

        public DemultiplexerQueue(object groupingKey, Action<IProcessingElement> enqueueToProcessingCallback)
        {
            _groupingKey = groupingKey;
            _enqueueToProcessingCallback = enqueueToProcessingCallback;
        }
        
        public bool Unblock()
        {
            if (_queue.Count > 0)
            {
                _enqueueToProcessingCallback(_queue.Dequeue());
                return true;
            }
            return false;
        }

        public bool Accepts(IProcessingElement fetchedElement)
        {
            return _groupingKey.Equals(fetchedElement.GroupingKey);
        }

        public void Enqueue(IProcessingElement processingElement)
        {
            _queue.Enqueue(processingElement);
        }
    }
}