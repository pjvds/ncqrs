using System.Collections.Generic;
using Ncqrs.Domain.Mapping;
using Ncqrs.Eventing;

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
