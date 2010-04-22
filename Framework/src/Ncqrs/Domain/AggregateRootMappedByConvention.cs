using System.Collections.Generic;
using Ncqrs.Domain.Mapping;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
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
