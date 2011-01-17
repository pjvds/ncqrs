using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// A event store. Can store and load events from an <see cref="IEventSource"/>.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Reads from the stream indicated from most recent snapshot, if any, up to and including the version specified
        /// or (if not specified) up to and including the most recent version.
        /// </summary>
        /// <remarks>
        /// Returned event stream contain a lastest snapshot if one exists.
        /// </remarks>
        /// <param name="id">The id of the event source that owns the events.</param>
        /// <param name="maxVersion">Maximum version number to be read.</param>
        /// <returns>All the events from the event source.</returns>
        CommittedEventStream ReadUntil(Guid id, long? maxVersion);

        /// <summary>
        /// Reads from the stream indicated from the revision specified until the end of the stream.
        /// </summary>
        /// <remarks>
        /// Returned event stream does not contain snapthots. This method is used when snapshots are stored in a separate store.
        /// </remarks>
        /// <param name="id">The id of the event source that owns the events.</param>
        /// <param name="minVersion">The minimum version number to be read.</param>
        /// <returns>All the events from the event source.</returns>
        CommittedEventStream ReadFrom(Guid id, long minVersion);

        /// <summary>
        /// Persist provided stream of events as a single and atomic commit.
        /// </summary>
        /// <exception cref="ConcurrencyException">Occurs when there is already a newer version of the event provider stored in the event store.</exception>
        /// <param name="eventStream">The stream of evnts to be persisted.</param>
        void Store(UncommittedEventStream eventStream);
    }    
}