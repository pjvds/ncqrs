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
            var sut = new PipelineProcessor(_eventStore, _pipelineStateStore, new SucceedingEventProcessor());
            sut.ProcessNext();

            _pipelineStateStore.AssertWasCalled(x => x.MarkLastProcessedEvent(_event));
        }

        [Test]
        public void Event_source_is_unblocked()
        {
            var sut = new PipelineProcessor(_eventStore, _pipelineStateStore, new SucceedingEventProcessor());
            sut.ProcessNext();

            _eventStore.AssertWasCalled(x => x.UnblockSource(_event.Event.EventSourceId));
        }
    }
}
