using System;
using Ncqrs.Domain.Mapping;

namespace Ncqrs.Domain
{
    public abstract class AggregateRootMappedWithAttributes : MappedAggregateRoot<AggregateRootMappedWithAttributes>
    {
        protected AggregateRootMappedWithAttributes()
            : base(new AttributeBasedDomainEventHandlerMappingStrategy())
        {
        }
    }
}
