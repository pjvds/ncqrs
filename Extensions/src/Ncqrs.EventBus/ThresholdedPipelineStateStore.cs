using System;

namespace Ncqrs.EventBus
{
    public class ThresholdedPipelineStateStore : IPipelineStateStore
    {
        private readonly IPipelineStateStore _wrappedStore;
        private readonly CursorPositionCalculator _cursorCalculator = new CursorPositionCalculator(0);
        private readonly int _threshold;

        public ThresholdedPipelineStateStore(IPipelineStateStore wrappedStore, int threshold)
        {
            _wrappedStore = wrappedStore;
            _threshold = threshold;
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

        public Guid? GetLastProcessedEvent()
        {
            return _wrappedStore.GetLastProcessedEvent();
        }
    }
}