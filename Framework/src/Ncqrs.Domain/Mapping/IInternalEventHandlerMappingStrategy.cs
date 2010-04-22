using System;
using System.Collections.Generic;

namespace Ncqrs.Domain.Mapping
{
    public interface IInternalEventHandlerMappingStrategy
    {
        IEnumerable<IInternalEventHandler> GetEventHandlersFromAggregateRoot(AggregateRoot aggregateRoot);
    }
}
