using System;

namespace Ncqrs.Eventing
{
    /// <summary>
    /// An event that is sourced by an event source. This event source represents all hist state changes via these events.
    /// </summary>
    public interface ISourcedEvent : IEvent
    {
        /// <summary>
        /// Gets the id of the event source that owns this event.
        /// </summary>
        /// <value>The id of the event source that owns this event.</value>
        Guid EventSourceId
        { 
            get;
        }

        /// <summary>
        /// Gets the event sequence number.
        /// </summary>
        /// <value>A number that represents the order of where this events occurred in the sequence.</value>
        long EventSequence
        { 
            get;
        }
    }
}
