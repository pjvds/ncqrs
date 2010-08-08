using System;

namespace Ncqrs.EventBus
{
    public class PipelineProcessor
    {
        private readonly IEventStore _eventStore;
        private readonly IPipelineStateStore _pipelineStateStore;
        private readonly IEventProcessor _eventProcessor;

        public PipelineProcessor(IEventStore eventStore, IPipelineStateStore pipelineStateStore, IEventProcessor eventProcessor)
        {
            _eventStore = eventStore;
            _eventProcessor = eventProcessor;
            _pipelineStateStore = pipelineStateStore;
        }

        public void ProcessNext()
        {
            var evnt = _eventStore.GetNext();
            try
            {
                _eventProcessor.Process(evnt.Event);
            }
            catch (Exception)
            {
                _pipelineStateStore.EnqueueForLaterProcessing(evnt.Event);
            }
            _pipelineStateStore.MarkLastProcessedEvent(evnt);
            _eventStore.UnblockSource(evnt.Event.EventSourceId);
        }
    }
}