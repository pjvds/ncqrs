using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus.Tests
{
    public class TestEvent : SourcedEvent
    {      
        public TestEvent()
        {            
        }

        public TestEvent(Guid eventIdentifier, Guid eventSourceIdentifier, long eventSequence, DateTime timeStamp) 
            : base(eventIdentifier, eventSourceIdentifier, eventSequence, timeStamp)
        {            
        }
    }
}