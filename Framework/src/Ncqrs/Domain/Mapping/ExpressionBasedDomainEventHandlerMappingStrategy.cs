using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Domain.Mapping
{
    /// <summary>
    /// A internal event handler mapping strategy that maps methods as an event handler based upon an expression statement.
    /// <remarks>
    /// </summary>
    public class ExpressionBasedDomainEventHandlerMappingStrategy : IDomainEventHandlerMappingStrategy<AggregateRootMappedWithExpressions>
    {
        public IEnumerable<IDomainEventHandler> GetEventHandlersFromAggregateRoot(AggregateRootMappedWithExpressions aggregateRoot)
        {
            throw new NotImplementedException("");
        }
    }
}