using Ncqrs.Eventing.Sourcing.Mapping;
using System;

namespace Ncqrs.Domain
{
    public abstract class AggregateRootMappedWithAttributes : MappedAggregateRoot
    {
        protected AggregateRootMappedWithAttributes()
            : base(new AttributeBasedEventHandlerMappingStrategy())
        {
        }

        protected AggregateRootMappedWithAttributes(Guid id)
            : base(id, new AttributeBasedEventHandlerMappingStrategy())
        {
        }
    }
}