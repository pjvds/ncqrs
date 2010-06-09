using System;
using Ncqrs.Domain.Mapping;

namespace Ncqrs.Domain
{
    public abstract class AggregateRootMappedWithAttributes : MappedAggregateRoot
    {
        protected AggregateRootMappedWithAttributes()
            : base(new AttributeBasedDomainEventHandlerMappingStrategy())
        {
        }
    }

    public abstract class EntityMappedWithAttributes : MappedEntity
    {
        protected EntityMappedWithAttributes()
            : base(new AttributeBasedDomainEventHandlerMappingStrategy())
        {
        }
    }
}