using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// A event store. Can store and load events from an <see cref="IEventSource"/>.
    /// </summary>
    [ContractClass(typeof(IEventStoreContracts))]
    public interface IEventStore
    {
        /// <summary>
        /// Get all events provided by an specified event provider.
        /// </summary>
        /// <param name="id">The id of the event source that owns the events.</param>
        /// <returns>All the events from the event source.</returns>
        IEnumerable<ISourcedEvent<IEventData>> GetAllEvents(Guid id);

        /// <summary>
        /// Get all events provided by an specified event source.
        /// </summary>
        /// <param name="eventSourceId">The id of the event source that owns the events.</param>
        /// <returns>All the events from the event source.</returns>
        IEnumerable<ISourcedEvent<IEventData>> GetAllEventsSinceVersion(Guid id, long version);

        /// <summary>
        /// Save all events from a specific event provider.
        /// </summary>
        /// <exception cref="ConcurrencyException">Occurs when there is already a newer version of the event provider stored in the event store.</exception>
        /// <param name="source">The source that should be saved.</param>
        void Save(IEventSource source);
    }

    [ContractClassFor(typeof(IEventStore))]
    internal class IEventStoreContracts : IEventStore
    {
        public IEnumerable<ISourcedEvent<IEventData>> GetAllEvents(Guid id)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ISourcedEvent<IEventData>>>() != null, "Result should never be null.");

            return default(IEnumerable<ISourcedEvent<IEventData>>);
        }

        public IEnumerable<ISourcedEvent<IEventData>> GetAllEventsSinceVersion(Guid id, long version)
        {
            Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<ISourcedEvent<IEventData>>>(), e => e.EventSequence > version));
            return default(IEnumerable<ISourcedEvent<IEventData>>);
        }

        public void Save(IEventSource source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "source cannot be null.");
        }

        public IEnumerable<ISourcedEvent<IEventData>> GetAllEventsSinceVersion(Guid id)
        {
            return default(IEnumerable<ISourcedEvent<IEventData>>);
        }
    }
}