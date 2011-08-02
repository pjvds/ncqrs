using System;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    /// <summary>
    /// An interface representing an event which can be published by an event bus (<see cref="IEventBus"/>).
    /// Possible implementations are:
    /// <list type="unordered">
    /// <item><see cref="CommittedEvent"/> which is used when processing/publishing events that were previously presisted.</item>
    /// <item><see cref="UncommittedEvent"/> which is used when processing/publishing events that were not previously presisted.</item>
    /// </list>
    /// </summary>
    public interface IPublishableEvent : IEvent
    {
        /// <summary>
        /// Gets the id of the event source that caused the event.
        /// </summary>
        /// <value>The id of the event source that caused the event.</value>
        Guid EventSourceId { get; }

        /// <summary>
        /// Gets the event sequence number.
        /// </summary>
        /// <remarks>
        /// An sequence of events always starts with <c>1</c>. So the first event in a sequence has the <see cref="EventSequence"/> value of <c>1</c>.
        /// </remarks>
        /// <value>A number that represents the order of where this events occurred in the sequence.</value>
        long EventSequence { get; }

        /// <summary>
        /// Id of the commit this event belongs to (usually corresponds to command id).
        /// </summary>
        Guid CommitId { get; }

        /// <summary>
        /// Gets the event payload.
        /// </summary>
        object Payload { get; }
    }    
}