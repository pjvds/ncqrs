using System.Collections.Generic;

namespace Ncqrs.Eventing.Sourcing.Mapping
{
    public interface ISourcedEventHandlerMappingStrategy
    {
        IEnumerable<ISourcedEventHandler> GetEventHandlersFromAggregateRoot(IEventSource eventSource);
    }
}