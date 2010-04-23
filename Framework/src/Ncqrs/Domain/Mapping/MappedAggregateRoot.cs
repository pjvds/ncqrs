using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Domain.Mapping
{
    public abstract class MappedAggregateRoot : AggregateRoot
    {
        private readonly IDomainEventHandlerMappingStrategy _mappingStrategy;

        protected MappedAggregateRoot(IDomainEventHandlerMappingStrategy strategy)
        {
            Contract.Requires<ArgumentNullException>(strategy != null, "The strategy cannot be null.");
            _mappingStrategy = strategy;
            InitializeHandlers();
        }

        protected MappedAggregateRoot(IDomainEventHandlerMappingStrategy strategy, IEnumerable<DomainEvent> history)
        {
            Contract.Requires<ArgumentNullException>(strategy != null, "The strategy cannot be null.");

            _mappingStrategy = strategy;

            InitializeHandlers();
            base.InitializeFromHistory(history);
        }

        private void InitializeHandlers()
        {
            foreach (var handler in _mappingStrategy.GetEventHandlersFromAggregateRoot(this))
            {
                RegisterHandler(handler);
            }
        }
    }
}
