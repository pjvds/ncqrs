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

        // TODO: "(T)(AggregateRoot)this)" is ugly bigtime but need the specific type in implementations of the aggregate mapper
        // so ill first work out my expression mapper and w'll continue work on this later
        private void InitializeHandlers()
        {
            foreach (var handler in _mappingStrategy.GetEventHandlersFromAggregateRoot((T)this))
                RegisterHandler(handler);
        }
    }
}