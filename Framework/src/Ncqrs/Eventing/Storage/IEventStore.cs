using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// A event store. Can store and load events from an <see cref="EventSource"/>.
    /// </summary>
    [ContractClass(typeof(IEventStoreContracts))]
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

    [ContractClassFor(typeof(IEventStore))]
    internal class IEventStoreContracts : IEventStore
    {
        public IEnumerable<HistoricalEvent> GetAllEventsForEventSource(Guid id)
        {
            Contract.Ensures(Contract.Result<IEnumerable<HistoricalEvent>>() != null, "Result should never be null.");

            return default(IEnumerable<HistoricalEvent>);
        }

        public IEnumerable<IEvent> Save(EventSource source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "source cannot be null.");
            Contract.Ensures(Contract.Result<IEnumerable<IEvent>>() != null, "Return should never be null.");

            return default(IEnumerable<IEvent>);
        }
    }
}