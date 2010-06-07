using System;
using System.Collections.Generic;

namespace Ncqrs.Domain.Mapping
{
    public abstract class DomainEventHandlerMappingStrategyMixin : IAggregateRootMixin
    {
        protected abstract IEnumerable<IDomainEventHandler> GetEventHandlersFromAggregateRoot(object aggregateRootInstance, Type aggregateRootPocoType);

        public void Initialize(Type aggregateRootPocoType, object aggregateRootPocoInstance)
        {
            InitializeHandlers((IAggregateRootInternal)aggregateRootPocoInstance, aggregateRootPocoType);
        }

        private void InitializeHandlers(IAggregateRootInternal aggregateRootInstance, Type aggregateRootPocoType)
        {
            foreach (var handler in GetEventHandlersFromAggregateRoot(aggregateRootInstance, aggregateRootPocoType))
            {
                aggregateRootInstance.RegisterHandler(handler);
            }
        }
    }
}