using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Storage
{
    public interface ISnapshotStore
    {
        ISnapshot GetLatestSnapshotForEventSource(Guid eventSourceId);

        /// <summary>
        /// Get all events provided by an specified event provider.
        /// </summary>
        /// <param name="id">The id of the event source that owns the events.</param>
        /// <returns>All the events from the event source.</returns>
        IEnumerable<ISourcedEvent> GetAllEventsForEventSource(Guid id);

        /// <summary>
        /// Save all events from a specific event provider.
        /// </summary>
        /// <exception cref="ConcurrencyException">Occurs when there is already a newer version of the event provider stored in the event store.</exception>
        /// <param name="source">The source that should be saved.</param>
        void Save(IEventSource source);
    }
}
