using System;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.EventBus.Tests
{
    public class PipelineProcessorSpecificationBase
    {        
        protected IProcessingElement _event;
        protected IEventQueue _eventQueue;
        protected IBrowsableElementStore _eventStore;

        [SetUp]
        public void SetUp()
        {
            _event = new SourcedEventProcessingElement(new UncommittedEvent(Guid.NewGuid(), Guid.NewGuid(), 0,0, DateTime.Now, new object(), new Version(1,0)));
            _eventStore = MockRepository.GenerateMock<IBrowsableElementStore>();
            _eventQueue = MockRepository.GenerateMock<IEventQueue>();
        }

        public class SucceedingEventProcessor : IElementProcessor
        {
            public void Process(IProcessingElement evnt)
            {
            }
        }

        public class FailingEventProcessor : IElementProcessor
        {
            public void Process(IProcessingElement evnt)
            {
                throw new Exception();
            }
        }
    }
}