using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Mapping
{
    public abstract class DomainEventHandlerMappingStrategy : IAggregateRootMixin
    {
        protected abstract IEnumerable<IDomainEventHandler> GetEventHandlersFromAggregateRoot(Type aggregateRootPocoType, object aggregateRootMixin);

        public void Initialize(Type aggregateRootPocoType, object aggregateRootMixin)
        {
            InitializeHandlers(aggregateRootPocoType, aggregateRootMixin);
        }

        private void InitializeHandlers(Type aggregateRootPocoType, object aggregateRootMixin)
        {
            var internalAggregateRoot = (IAggregateRootInternal) aggregateRootMixin;
            foreach (var handler in GetEventHandlersFromAggregateRoot(aggregateRootPocoType, aggregateRootMixin))
            {
                internalAggregateRoot.RegisterHandler(handler);
            }
        }
    }
}