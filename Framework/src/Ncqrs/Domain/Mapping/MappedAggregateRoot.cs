using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Mapping
{
    public abstract class MappedAggregateRoot<T> : AggregateRoot where T : MappedAggregateRoot<T>
    {
        private readonly IDomainEventHandlerMappingStrategy<T> _mappingStrategy;

        protected MappedAggregateRoot(IDomainEventHandlerMappingStrategy<T> strategy)
        {
            Contract.Requires<ArgumentNullException>(strategy != null, "The strategy cannot be null.");

            _mappingStrategy = strategy;
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            foreach (var handler in _mappingStrategy.GetEventHandlersFromAggregateRoot((T)this))
                RegisterHandler(handler);
        }
    }
}