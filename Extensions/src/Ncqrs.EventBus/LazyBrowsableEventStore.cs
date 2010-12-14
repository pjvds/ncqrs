using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus
{
    public class LazyBrowsableEventStore : IBrowsableEventStore
    {
        private readonly IBrowsableEventStore _wrappedStore;
        private readonly CursorPositionCalculator _cursorCalculator = new CursorPositionCalculator(0);
        private readonly int _threshold;

        public LazyBrowsableEventStore(IBrowsableEventStore wrappedStore, int threshold)
        {
            _wrappedStore = wrappedStore;
            _threshold = threshold;
        }

        public IEnumerable<SourcedEvent> FetchEvents(int maxCount)
        {
            return _wrappedStore.FetchEvents(maxCount);
        }

        public void MarkLastProcessedEvent(SequencedEvent evnt)
        {
            _cursorCalculator.Append(evnt);
            if (_cursorCalculator.SequenceLength >= _threshold)
            {
                _wrappedStore.MarkLastProcessedEvent(evnt);
                _cursorCalculator.ClearSequence();
            }
        }
    }
}