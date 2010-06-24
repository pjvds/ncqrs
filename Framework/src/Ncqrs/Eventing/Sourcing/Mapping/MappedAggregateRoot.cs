using System;
using System.Diagnostics.Contracts;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Mapping
{
    public abstract class MappedAggregateRoot : AggregateRoot
    {
        [NonSerialized] 
        private readonly ISourcedEventHandlerMappingStrategy _mappingStrategy;

        protected MappedAggregateRoot(ISourcedEventHandlerMappingStrategy strategy)
        {
            Contract.Requires<ArgumentNullException>(strategy != null, "The strategy cannot be null.");

            _mappingStrategy = strategy;
            InitializeHandlers();
        }

        protected void InitializeHandlers()
        {
            foreach (var handler in _mappingStrategy.GetEventHandlersFromAggregateRoot(this))
                RegisterHandler(handler);
        }
    }
}