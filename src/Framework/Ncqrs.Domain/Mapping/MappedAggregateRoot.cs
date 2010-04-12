using System;
using System.Collections.Generic;

namespace Ncqrs.Domain.Mapping
{
    public abstract class MappedAggregateRoot : AggregateRoot
    {
        private bool _handlersInitialized = false;

        protected MappedAggregateRoot(IUniqueIdentifierGenerator idGenerator) : base(idGenerator)
        {
            InitializeHandlers();
        }

        protected MappedAggregateRoot(IEnumerable<HistoricalEvent> history) : base()
        {
            InitializeHandlers();
            base.InitializeFromHistory(history);
        }

        private void InitializeHandlers()
        {
            var handlerFactory = new EventHandlerFactory();
            foreach (var x in handlerFactory.CreateHandlers(this))
            {
                RegisterHandler(x.Key, x.Value);
            }

            _handlersInitialized = true;
        }
    }
}
