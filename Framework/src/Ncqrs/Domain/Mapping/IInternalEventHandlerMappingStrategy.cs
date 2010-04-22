using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing.ServiceModel.Bus.Mapping
{
    public interface IInternalEventHandlerMappingStrategy
    {
        IEnumerable<IInternalEventHandler> GetEventHandlersFromAggregateRoot(AggregateRoot aggregateRoot);
    }
}
