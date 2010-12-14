using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.EventBus.Tests
{
    [TestFixture]
    public class When_processing_fails : PipelineProcessorSpecificationBase
    {             
        [Test]
        public void Event_is_marked_as_processed()
        {
            var sut = CreateProcessor();

            sut.ProcessNext(_event);

            _pipelineStateStore.AssertWasCalled(x => x.MarkLastProcessedEvent(_event));
        }

        private PipelineProcessor CreateProcessor()
        {
            var pipelineProcessor = new PipelineProcessor(new FailingEventProcessor());
            pipelineProcessor.EventProcessed += (s, e) => _pipelineStateStore.MarkLastProcessedEvent(e.Event);
            return pipelineProcessor;
        }
    }
}