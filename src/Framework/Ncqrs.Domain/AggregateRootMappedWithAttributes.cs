using System;
using System.Collections.Generic;
using Ncqrs.Domain.Mapping;

namespace Ncqrs.Domain
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
