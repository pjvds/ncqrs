using System;
using System.Collections.Generic;

namespace Ncqrs.Domain.Mapping
{
    public interface IDomainEventHandlerMappingStrategy
    {
        IEnumerable<IDomainEventHandler> GetEventHandlersFromAggregateRoot(object instance);
    }
}