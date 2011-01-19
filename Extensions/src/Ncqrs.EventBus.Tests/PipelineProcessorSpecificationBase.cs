using System;
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
            _event = new FakeProcessingElement();
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