using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Mapping
{
    public abstract class MappedAggregateRoot : AggregateRoot
    {
        private readonly IInternalEventHandlerMappingStrategy _mappingStrategy;

        protected MappedAggregateRoot(IInternalEventHandlerMappingStrategy strategy)
            : base()
        {
            Contract.Requires<ArgumentNullException>(strategy != null, "The strategy cannot be null.");
            _mappingStrategy = strategy;
            InitializeHandlers();
        }

        protected MappedAggregateRoot(IInternalEventHandlerMappingStrategy strategy, IUniqueIdentifierGenerator idGenerator)
            : base(idGenerator)
        {
            _mappingStrategy = strategy;
            InitializeHandlers();
        }

        protected MappedAggregateRoot(IInternalEventHandlerMappingStrategy strategy, IEnumerable<HistoricalEvent> history)
            : base()
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
