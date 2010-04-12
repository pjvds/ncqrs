using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Mapping;

namespace Ncqrs.Domain
{
    public abstract class AggregateRootMappedByConvention : MappedAggregateRoot
    {
        protected AggregateRootMappedByConvention()
            : base(new ConventionBasedMappingStrategy())
        {
        }

        protected AggregateRootMappedByConvention(IUniqueIdentifierGenerator idGenerator) : base(new ConventionBasedMappingStrategy(), idGenerator)
        {
        }

        protected AggregateRootMappedByConvention(IEnumerable<HistoricalEvent> history) : base(new ConventionBasedMappingStrategy(), history)
        {
        }
    }
}
