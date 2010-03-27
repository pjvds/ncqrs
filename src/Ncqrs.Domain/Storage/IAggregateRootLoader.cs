using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing;

namespace Ncqrs.Domain.Storage
{
    public interface IAggregateRootLoader
    {
        AggregateRoot LoadAggregateRootFromEvents(Type aggregateRootType, IEnumerable<HistoricalEvent> events);
    }
}
