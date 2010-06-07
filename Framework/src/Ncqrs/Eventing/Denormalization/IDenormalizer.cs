using System;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Eventing.Denormalization
{
    public interface IDenormalizer<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
    {      
    }
}