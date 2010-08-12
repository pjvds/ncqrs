using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.EventBus.Tests
{
    [TestFixture]
    public class When_processing_succeeds : PipelineProcessorSpecificationBase
    {
        [Test]
        public void Event_is_marked_as_processed()
        {
            var sut = CreateProcessor();
            sut.ProcessNext(_event);

            _pipelineStateStore.AssertWasCalled(x => x.MarkLastProcessedEvent(_event));
        }

        [Test]
        public void Event_source_is_unblocked()
        {
            var sut = CreateProcessor();
            sut.ProcessNext(_event);

            _eventQueue.AssertWasCalled(x => x.MarkAsProcessed(_event));
        }

        private PipelineProcessor CreateProcessor()
        {
            return new PipelineProcessor(
                _pipelineBackupQueue,
                new SucceedingEventProcessor(),
                _eventQueue,
                x => _pipelineStateStore.MarkLastProcessedEvent(x));
        }
    }
}
