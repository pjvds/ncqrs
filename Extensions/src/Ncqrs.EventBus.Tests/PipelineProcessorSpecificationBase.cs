using System;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.EventBus.Tests
{
    public class PipelineProcessorSpecificationBase
    {        
        protected SequencedEvent _event;
        protected IEventQueue _eventQueue;
        protected IBrowsableEventStore _eventStore;
        protected IPipelineStateStore _pipelineStateStore;

        [SetUp]
        public void SetUp()
        {
            _event = new SequencedEvent(1, new TestEvent());
            _eventStore = MockRepository.GenerateMock<IBrowsableEventStore>();
            _pipelineStateStore = MockRepository.GenerateMock<IPipelineStateStore>();
            _eventQueue = MockRepository.GenerateMock<IEventQueue>();
        }

        public class SucceedingEventProcessor : IEventProcessor
        {
            public void Process(SourcedEvent evnt)
            {
            }
        }

        public class FailingEventProcessor : IEventProcessor
        {
            public void Process(SourcedEvent evnt)
            {
                throw new Exception();
            }
        }
    }
}