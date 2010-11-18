using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus.IntegrationTest
{
    public class RandomEvent : SourcedEvent
    {      
        public RandomEvent()
        {            
        }

        public RandomEvent(long sequence)
            : base(Guid.NewGuid(), Guid.NewGuid(), sequence, DateTime.UtcNow)
        {                        
        }

        public RandomEvent(Guid sourceId, long sequence)
            : base(Guid.NewGuid(), sourceId, sequence, DateTime.UtcNow)
        {
        }
    }
}