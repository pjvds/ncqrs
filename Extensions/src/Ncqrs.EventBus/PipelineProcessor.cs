using System;

namespace Ncqrs.EventBus
{
    public class PipelineProcessor
    {
        private readonly IPipelineBackupQueue _pipelineBackupQueue;
        private readonly IEventProcessor _eventProcessor;
        private readonly IEventQueue _eventQueue;
        private readonly Action<SequencedEvent> _postProcessingAction;

        public PipelineProcessor(
            IPipelineBackupQueue pipelineBackupQueue, 
            IEventProcessor eventProcessor,
            IEventQueue eventQueue,
            Action<SequencedEvent> postProcessingAction)
        {
            _pipelineBackupQueue = pipelineBackupQueue;
            _eventProcessor = eventProcessor;
            _eventQueue = eventQueue;
            _postProcessingAction = postProcessingAction;
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
                _postProcessingAction(evnt);
                _eventQueue.MarkAsProcessed(evnt);
            }
        }
    }
}