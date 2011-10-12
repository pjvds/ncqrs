using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncqrs.EventBus
{
    public class InMemoryBufferedBrowsableElementStore : IBrowsableElementStore
    {
        private readonly object _lock = new object();
        private bool _inMemoryMode;
        private IProcessingElement _firstElementPushed;
        private readonly FixedSizeChunkBuffer _buffer;
        private readonly IBrowsableElementStore _persistentStore;
        private readonly int _fetchSize;

        public InMemoryBufferedBrowsableElementStore(IBrowsableElementStore persistentStore, int fetchSize)
        {
            _persistentStore = persistentStore;
            _fetchSize = fetchSize;
            _buffer = new FixedSizeChunkBuffer(fetchSize);
        }

        public IEnumerable<IProcessingElement> Fetch(string pipelineName, int maxCount)
        {
            if (_fetchSize != maxCount)
            {
                throw new NotSupportedException("Buffered element store can only return elements in fixed-size batches.");
            }
            lock (_lock)
            {
                return _inMemoryMode 
                    ? FetchFromBuffer() 
                    : FetchFromPersistentStoreAndCorrelateWithBuffer(pipelineName);
            }
        }

        private IEnumerable<IProcessingElement> FetchFromBuffer()
        {
            return _buffer.TakeOne();
        }

        private IEnumerable<IProcessingElement> FetchFromPersistentStoreAndCorrelateWithBuffer(string pipelineName)
        {
            // We need to copy the fetched items to prevent fetchedFromPersistentStore.Any()
            // and/or CorrelateWithBufffer consuming events and not returning them.
            // Rx extensions MemoizeAll would be a good solution, but I'm not sure we want
            // an extra dependency, ToList is fine as long as _fetchSize is reasonable.
            var fetchedFromPersistentStore = _persistentStore.Fetch(pipelineName, _fetchSize).ToList();
            if (!fetchedFromPersistentStore.Any() || CorrelateWithBufffer(fetchedFromPersistentStore))
            {
                _inMemoryMode = true;
            }
            return fetchedFromPersistentStore;
        }

        private bool CorrelateWithBufffer(IEnumerable<IProcessingElement> fetchedFromPersistentStore)
        {
            return IsFirstElementPushed() 
                && FirstPushedElementMatchesOneOfFetched(fetchedFromPersistentStore);
        }

        private bool FirstPushedElementMatchesOneOfFetched(IEnumerable<IProcessingElement> fetchedFromPersistentStore)
        {
            return fetchedFromPersistentStore.Any(x => x.UniqueId == _firstElementPushed.UniqueId);
        }

        private bool IsFirstElementPushed()
        {
            return _firstElementPushed != null;
        }

        public void MarkLastProcessedElement(string pipelineName, IProcessingElement processingElement)
        {
            _persistentStore.MarkLastProcessedElement(pipelineName, processingElement);
        }

        public void PushElement(IProcessingElement processingElement)
        {
            lock (_lock)
            {
                _buffer.Append(processingElement);
                EnsureFirstPushedElementMarked(processingElement);    
            }
        }

        private void EnsureFirstPushedElementMarked(IProcessingElement processingElement)
        {
            if (_firstElementPushed == null)
            {
                _firstElementPushed = processingElement;
            }
        }
    }
}
