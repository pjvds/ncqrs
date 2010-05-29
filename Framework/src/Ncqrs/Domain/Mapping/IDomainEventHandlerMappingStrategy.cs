using System;
using System.Collections.Generic;

namespace Ncqrs.Domain.Mapping
{
    public interface IDomainEventHandlerMappingStrategy<T> where T : AggregateRoot
    {
        IEnumerable<IDomainEventHandler> GetEventHandlersFromAggregateRoot(T aggregateRoot);
    }
}