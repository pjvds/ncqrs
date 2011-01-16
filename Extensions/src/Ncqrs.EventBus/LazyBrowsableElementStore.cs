using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public class LazyBrowsableElementStore : IBrowsableElementStore
    {
        private const int DefaultThreshold = 1;

        private readonly IBrowsableElementStore _wrappedStore;
        private readonly CursorPositionCalculator _cursorCalculator = new CursorPositionCalculator(0);
        private readonly int _threshold;

        public LazyBrowsableElementStore(IBrowsableElementStore wrappedStore, int threshold = DefaultThreshold)
        {
            _wrappedStore = wrappedStore;
            _threshold = threshold;
        }

        public IEnumerable<IProcessingElement> Fetch(int maxCount)
        {
            return _wrappedStore.Fetch(maxCount);
        }

        public void MarkLastProcessedEvent(IProcessingElement processingElement)
        {
            _cursorCalculator.Append(processingElement);
            if (_cursorCalculator.SequenceLength >= _threshold)
            {
                _wrappedStore.MarkLastProcessedEvent(processingElement);
                _cursorCalculator.ClearSequence();
            }
        }
    }
}