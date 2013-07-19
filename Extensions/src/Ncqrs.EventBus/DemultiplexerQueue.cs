using System;
using System.Collections.Generic;

namespace Ncqrs.EventBus
{
    public class DemultiplexerQueue
    {
        private readonly object _groupingKey;      
        private readonly Queue<IProcessingElement> _queue = new Queue<IProcessingElement>();

        public DemultiplexerQueue(object groupingKey)
        {
            _groupingKey = groupingKey;          
        }

        public bool Accepts(IProcessingElement fetchedElement)
        {
            return _groupingKey.Equals(fetchedElement.GroupingKey);
        }

        public void Enqueue(IProcessingElement processingElement)
        {
            _queue.Enqueue(processingElement);
        }

        public bool IsEmpty()
        {
            return (_queue.Count == 0);
        }
    }
}