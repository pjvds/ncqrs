using System;
using Ncqrs.Domain.Mapping;

namespace Ncqrs.Domain
{
    public abstract class AggregateRootMappedWithAttributes : MappedAggregateRoot<AggregateRootMappedWithAttributes>
    {        
        public override void Initialize(object aggregateRootInstance)
        {
            MappingStrategy = new AttributeBasedDomainEventHandlerMappingStrategy();
            base.Initialize(aggregateRootInstance);
        }
    }
}