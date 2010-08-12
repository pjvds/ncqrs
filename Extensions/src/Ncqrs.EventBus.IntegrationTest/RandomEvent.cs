using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.EventBus.IntegrationTest
{
    public class RandomEvent : SourcedEvent
    {      
        public RandomEvent()
            : base(Guid.NewGuid(), Guid.NewGuid(), 0, DateTime.Now)
        {            
        }

    }
}