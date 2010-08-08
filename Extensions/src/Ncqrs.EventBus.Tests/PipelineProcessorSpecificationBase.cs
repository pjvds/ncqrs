using System;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.EventBus.Tests
{
    public class PipelineProcessorSpecificationBase
    {
        protected SequencedEvent _event;
        protected IEventStore _eventStore;
        protected IPipelineStateStore _pipelineStateStore;

        [SetUp]
        public void SetUp()
        {
            _event = new SequencedEvent(1, new TestEvent());
            _eventStore = MockRepository.GenerateMock<IEventStore>();
            _eventStore.Expect(x => x.GetNext()).Repeat.Once().Return(_event);

            _pipelineStateStore = MockRepository.GenerateMock<IPipelineStateStore>();
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