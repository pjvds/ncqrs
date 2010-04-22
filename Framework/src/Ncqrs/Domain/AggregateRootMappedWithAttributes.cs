using System;
using System.Collections.Generic;
using Ncqrs.Eventing.ServiceModel.Bus.Mapping;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public abstract class AggregateRootMappedWithAttributes : MappedAggregateRoot
    {
        protected AggregateRootMappedWithAttributes()
            : base(new AttributeBasedInternalEventHandlerMappingStrategy())
        {
        }

        protected AggregateRootMappedWithAttributes(IUniqueIdentifierGenerator idGenerator)
            : base(new AttributeBasedInternalEventHandlerMappingStrategy(), idGenerator)
        {
        }

        protected AggregateRootMappedWithAttributes(IEnumerable<HistoricalEvent> history)
            : base(new AttributeBasedInternalEventHandlerMappingStrategy(), history)
        {
        }
    }
}
