using System;

namespace Ncqrs.Eventing
{
    public interface IEventSourcedEvent : IEvent
    {
        Guid EventSourceId
        { 
            get;
        }
    }
}
