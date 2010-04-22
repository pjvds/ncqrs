using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Domain.Mapping
{
    public abstract class MappedAggregateRoot : AggregateRoot
    {
        private readonly IInternalEventHandlerMappingStrategy _mappingStrategy;

        protected MappedAggregateRoot(IInternalEventHandlerMappingStrategy strategy)
        {
            Contract.Requires<ArgumentNullException>(strategy != null, "The strategy cannot be null.");
            _mappingStrategy = strategy;
            InitializeHandlers();
        }

        protected MappedAggregateRoot(IInternalEventHandlerMappingStrategy strategy, IEnumerable<DomainEvent> history)
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
