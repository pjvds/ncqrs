using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Mapping
{
    public abstract class MappedEntity : Entity
    {
        [NonSerialized]
        private readonly IDomainEventHandlerMappingStrategy _mappingStrategy;

        protected MappedEntity(IDomainEventHandlerMappingStrategy strategy)
        {
            Contract.Requires<ArgumentNullException>(strategy != null, "The strategy cannot be null.");

            _mappingStrategy = strategy;
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            foreach (var handler in _mappingStrategy.GetEventHandlersFromAggregateRoot(this))
                RegisterHandler(handler);
        }
    }
}