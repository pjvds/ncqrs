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
            var sut = new PipelineProcessor(_eventStore, _pipelineStateStore, new FailingEventProcessor());
            sut.ProcessNext();

            _pipelineStateStore.AssertWasCalled(x => x.EnqueueForLaterProcessing(_event.Event));
        }

        [Test]
        public void Event_is_marked_as_processed()
        {
            var sut = new PipelineProcessor(_eventStore, _pipelineStateStore, new FailingEventProcessor());
            sut.ProcessNext();

            _pipelineStateStore.AssertWasCalled(x => x.MarkLastProcessedEvent(_event));
        }
    }
}