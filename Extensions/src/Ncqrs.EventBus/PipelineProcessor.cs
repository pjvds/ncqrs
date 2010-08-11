using System;

namespace Ncqrs.EventBus
{
    public class PipelineProcessor
    {
        private readonly IPipelineStateStore _pipelineStateStore;
        private readonly IPipelineBackupQueue _pipelineBackupQueue;
        private readonly IEventProcessor _eventProcessor;
        private readonly IEventQueue _eventQueue;

        public PipelineProcessor(
            IPipelineBackupQueue pipelineBackupQueue, 
            IPipelineStateStore pipelineStateStore, 
            IEventProcessor eventProcessor,
            IEventQueue eventQueue)
        {
            _pipelineBackupQueue = pipelineBackupQueue;
            _eventProcessor = eventProcessor;
            _eventQueue = eventQueue;
            _pipelineStateStore = pipelineStateStore;
        }

        public void ProcessNext(SequencedEvent evnt)
        {
            try
            {
                _eventProcessor.Process(evnt.Event);
            }
            catch (Exception)
            {
                _pipelineBackupQueue.EnqueueForLaterProcessing(evnt.Event);
            }
            finally
            {
                _pipelineStateStore.MarkLastProcessedEvent(evnt);
                _eventQueue.MarkAsProcessed(evnt);
            }
        }
    }
}