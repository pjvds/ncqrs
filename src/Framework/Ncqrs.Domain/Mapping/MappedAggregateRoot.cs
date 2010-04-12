using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Mapping
{
    public abstract class MappedAggregateRoot : AggregateRoot
    {
        private readonly IMappingStrategy _mappingStrategy;

        protected MappedAggregateRoot(IMappingStrategy strategy)
            : base()
        {
            Contract.Requires<ArgumentNullException>(strategy != null, "The strategy cannot be null.");
            _mappingStrategy = strategy;
            InitializeHandlers();
        }

        protected MappedAggregateRoot(IMappingStrategy strategy, IUniqueIdentifierGenerator idGenerator)
            : base(idGenerator)
        {
            _mappingStrategy = strategy;
            InitializeHandlers();
        }

        protected MappedAggregateRoot(IMappingStrategy strategy, IEnumerable<HistoricalEvent> history)
            : base()
        {
            Contract.Requires<ArgumentNullException>(strategy != null, "The strategy cannot be null.");

            _mappingStrategy = strategy;

            InitializeHandlers();
            base.InitializeFromHistory(history);
        }

        private void InitializeHandlers()
        {
            foreach (var x in _mappingStrategy.GetEventHandlersFromAggregateRoot(this))
            {
                RegisterHandler(x.Item1, x.Item2);
            }
        }
    }
}
