using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing;

namespace Ncqrs.Domain.Storage
{
    public class DefaultAggregateRootLoader : IAggregateRootLoader
    {
        public AggregateRoot LoadAggregateRootFromEvents(Type aggregateRootType, IEnumerable<HistoricalEvent> events)
        {
            return (AggregateRoot)Activator.CreateInstance(aggregateRootType, events);
        }
    }
}
