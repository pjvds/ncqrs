using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing;

namespace Ncqrs.Domain.Storage
{
    /// <summary>
    /// The default aggregate root loader than can load an aggregate root instance from a historic event stream.
    /// </summary>
    public class DefaultAggregateRootLoader : IAggregateRootLoader
    {
        public AggregateRoot LoadAggregateRootFromEvents(Type aggregateRootType, IEnumerable<HistoricalEvent> events)
        {
            return (AggregateRoot)Activator.CreateInstance(aggregateRootType, events);
        }
    }
}