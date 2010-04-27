using System;

namespace Ncqrs.Eventing
{
    public interface ISourcedEvent : IEvent
    {
        Guid EventSourceId
        { 
            get;
        }

        long EventSequence
        { 
            get;
        }
    }
}
