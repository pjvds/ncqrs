using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.EventBus.Tests
{
    [TestFixture]
    public class When_processing_fails : PipelineProcessorSpecificationBase
    {
        [Test]
        public void Event_is_enqueue()
        {
            var sut = CreateProcessor();

            sut.ProcessNext(_event);

            _pipelineBackupQueue.AssertWasCalled(x => x.EnqueueForLaterProcessing(_event.Event));
        }        

        [Test]
        public void Event_is_marked_as_processed()
        {
            var sut = CreateProcessor();

            sut.ProcessNext(_event);

            _pipelineStateStore.AssertWasCalled(x => x.MarkLastProcessedEvent(_event));
        }

        private PipelineProcessor CreateProcessor()
        {
            return new PipelineProcessor(
                _pipelineBackupQueue,
                new FailingEventProcessor(),
                MockRepository.GenerateMock<IEventQueue>(),
                x => _pipelineStateStore.MarkLastProcessedEvent(x));
        }
    }
}