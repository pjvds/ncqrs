using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// A event store. Can store and load events from an <see cref="EventSource"/>.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Get all events provided by an specified event provider.
        /// </summary>
        /// <param name="id">The id of the event source that owns the events.</param>
        /// <returns>All the events from the event source.</returns>
        IEnumerable<HistoricalEvent> GetAllEventsForEventSource(Guid id);

        /// <summary>
        /// Save all events from a specific event provider.
        /// </summary>
        /// <exception cref="ConcurrencyException">Occurs when there is already a newer version of the event provider stored in the event store.</exception>
        /// <param name="source">The source that should be saved.</param>
        IEnumerable<IEvent> Save(EventSource source);
    }
}