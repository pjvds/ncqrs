using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Domain.Storage
{
    /// <summary>
    /// Can load an aggregate root from a stream of historical events.
    /// </summary>
    [ContractClass(typeof(IAggregateRootLoaderContracts))]
    public interface IAggregateRootLoader
    {
        /// <summary>
        /// Loads the aggregate root from historical events.
        /// </summary>
        /// <param name="aggregateRootType">Type of the aggregate root to load.</param>
        /// <param name="events">The historical events.</param>
        /// <exception cref="AggregateLoaderException">Occurs when the aggregate root could not be loaded.</exception>
        /// <returns>A new instance of the specified aggregate root type loaded with context that has been build from the events.</returns>
        AggregateRoot LoadAggregateRootFromEvents(Type aggregateRootType, IEnumerable<HistoricalEvent> events);
    }

    [ContractClassFor(typeof(IAggregateRootLoader))]
    internal sealed class IAggregateRootLoaderContracts : IAggregateRootLoader
    {
        public AggregateRoot LoadAggregateRootFromEvents(Type aggregateRootType, IEnumerable<HistoricalEvent> events)
        {
            Contract.Requires<ArgumentNullException>(aggregateRootType != null);
            Contract.Requires<ArgumentNullException>(events != null);

            return default(AggregateRoot);
        }
    }
}
