using System;
using System.Collections.Generic;
using Ncqrs.Eventing.ServiceModel.Bus.Mapping;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public abstract class AggregateRootMappedByConvention : MappedAggregateRoot
    {
        protected AggregateRootMappedByConvention()
            : base(new ConventionBasedInternalEventHandlerMappingStrategy())
        {
        }

        protected AggregateRootMappedByConvention(IUniqueIdentifierGenerator idGenerator) : base(new ConventionBasedInternalEventHandlerMappingStrategy(), idGenerator)
        {
        }

        protected AggregateRootMappedByConvention(IEnumerable<HistoricalEvent> history) : base(new ConventionBasedInternalEventHandlerMappingStrategy(), history)
        {
        }
    }
}
