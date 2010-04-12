using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Domain.Mapping
{
    public interface IMappingStrategy
    {
        IEnumerable<Tuple<Type, Action<IEvent>>> GetEventHandlersFromAggregateRoot(AggregateRoot aggregateRoot);
    }
}
