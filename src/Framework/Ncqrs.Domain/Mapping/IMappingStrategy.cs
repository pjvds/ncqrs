using System;
using System.Collections.Generic;

namespace Ncqrs.Domain.Mapping
{
    public interface IMappingStrategy
    {
        IEnumerable<IInternalEventHandler> GetEventHandlersFromAggregateRoot(AggregateRoot aggregateRoot);
    }
}
