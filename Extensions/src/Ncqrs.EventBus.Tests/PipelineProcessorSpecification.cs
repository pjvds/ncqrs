using System;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.EventBus.Tests
{
    public class PipelineProcessorSpecification
    {        
        private IProcessingElement _element;

        [SetUp]
        public void SetUp()
        {
            _element = new TestElement();
        }

        [Test]
        public void When_processing_succeeds_processed_event_is_fired()
        {
            bool eventFired = false;
            var pipelineProcessor = new PipelineProcessor(new SucceedingEventProcessor());
            pipelineProcessor.EventProcessed += (s, e) => eventFired = true;
            
            pipelineProcessor.ProcessNext(_element);

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void When_processing_fails_processed_event_is_not_fired()
        {
            bool eventFired = false;
            var pipelineProcessor = new PipelineProcessor(new FailingEventProcessor());
            pipelineProcessor.EventProcessed += (s, e) => eventFired = true;

            pipelineProcessor.ProcessNext(_element);

            Assert.IsFalse(eventFired);
        }

        public class SucceedingEventProcessor : IElementProcessor
        {
            public void Process(IProcessingElement element)
            {
            }
        }

        public class FailingEventProcessor : IElementProcessor
        {
            public void Process(IProcessingElement element)
            {
                throw new Exception();
            }
        }

        private class TestElement : IProcessingElement
        {
            public int SequenceNumber
            {
                get; set;
            }

            public string UniqueId
            {
                get { return "ID"; }
            }

            public object GroupingKey
            {
                get { return "Key"; }
            }
        }
    }
}