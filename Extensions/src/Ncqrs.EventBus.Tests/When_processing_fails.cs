using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.EventBus.Tests
{
    [TestFixture]
    public class When_processing_fails : PipelineProcessorSpecificationBase
    {             
        [Test]
        public void Event_is_not_marked_as_processed()
        {
            var sut = CreateProcessor();

            sut.ProcessNext(_event);

            _eventStore.AssertWasNotCalled(x => x.MarkLastProcessedElement(_event));
        }

        private PipelineProcessor CreateProcessor()
        {
            var pipelineProcessor = new PipelineProcessor(new FailingEventProcessor());
            pipelineProcessor.EventProcessed += (s, e) => _eventStore.MarkLastProcessedElement(e.ProcessedElement);
            return pipelineProcessor;
        }
    }
}