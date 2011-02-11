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
        public void OnProcessed_event_is_fired()
        {
            var sut = CreateProcessor();
            sut.ProcessNext(_event);

            _eventStore.AssertWasCalled(x => x.MarkLastProcessedElement(_event));
        }        

        private PipelineProcessor CreateProcessor()
        {
            var pipelineProcessor = new PipelineProcessor(new SucceedingEventProcessor());
            pipelineProcessor.EventProcessed += (s, e) => _eventStore.MarkLastProcessedElement(e.ProcessedElement);
            return pipelineProcessor;
        }
    }
}
