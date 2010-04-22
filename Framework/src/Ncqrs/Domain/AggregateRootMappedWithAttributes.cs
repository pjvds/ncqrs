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

        protected AggregateRootMappedWithAttributes(IEnumerable<DomainEvent> history)
            : base(new AttributeBasedInternalEventHandlerMappingStrategy(), history)
        {
        }
    }
}
