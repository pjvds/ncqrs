using System.Collections.Generic;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Domain.Mapping
{
    public interface IInternalEventHandlerMappingStrategy
    {
        IEnumerable<IInternalEventHandler> GetEventHandlersFromAggregateRoot(AggregateRoot aggregateRoot);
    }
}
