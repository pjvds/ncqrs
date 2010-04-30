using System;

namespace Ncqrs.Eventing
{
    public interface ISourcedEvent : IEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event.
        /// </summary>
        /// <value></value>
        Guid EventIdentifier
        {
            get;
        }

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
