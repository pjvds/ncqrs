using System;
using System.Collections.Generic;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Domain.Mapping
{
    public interface ISourcedEventHandlerMappingStrategy
    {
        IEnumerable<IDomainEventHandler> GetEventHandlersFromAggregateRoot(IEventSource eventSource);
    }
}