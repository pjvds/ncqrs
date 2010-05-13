using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

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
        IEnumerable<ISourcedEvent> GetAllEvents(Guid id);

        /// <summary>
        /// Get all events provided by an specified event source.
        /// </summary>
        /// <param name="eventSourceId">The id of the event source that owns the events.</param>
        /// <returns>All the events from the event source.</returns>
        IEnumerable<ISourcedEvent> GetAllEventsSinceVersion(Guid id, long version);

        /// <summary>
        /// Save all events from a specific event provider.
        /// </summary>
        /// <exception cref="ConcurrencyException">Occurs when there is already a newer version of the event provider stored in the event store.</exception>
        /// <param name="source">The source that should be saved.</param>
        void Save(IEventSource source);

        /// <summary>
        /// Saves a snapshot of the specified event source.
        /// </summary>
        void SaveShapshot(ISnapshot source);

        /// <summary>
        /// Gets a snapshot of a particular event source, if one exists. Otherwise, returns <c>null</c>.
        /// </summary>
        ISnapshot GetSnapshot(Guid eventSourceId);
    }

    [ContractClassFor(typeof(IEventStore))]
    internal class IEventStoreContracts : IEventStore
    {
        public IEnumerable<ISourcedEvent> GetAllEvents(Guid id)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ISourcedEvent>>() != null, "Result should never be null.");

            return default(IEnumerable<ISourcedEvent>);
        }

        public IEnumerable<ISourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<ISourcedEvent>>(), e => e.EventSequence > version));
            return default(IEnumerable<ISourcedEvent>);
        }

        public void Save(IEventSource source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "source cannot be null.");
        }

        public void SaveShapshot(ISnapshot source)
        {
        }

        public IEnumerable<ISourcedEvent> GetAllEventsSinceVersion(Guid id)
        {
            return default(IEnumerable<ISourcedEvent>);
        }

        public void SaveshapShot(ISnapshot source)
        {
        }

        public ISnapshot GetSnapshot(Guid eventSourceId)
        {
            Contract.Ensures(Contract.Result<ISnapshot>() != null ? Contract.Result<ISnapshot>().EventSourceId == eventSourceId : true);

            return default(ISnapshot);
        }
    }
}