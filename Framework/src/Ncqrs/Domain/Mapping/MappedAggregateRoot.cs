using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Mapping
{
    public abstract class MappedAggregateRoot<T> : AggregateRoot where T : MappedAggregateRoot<T>
    {
        protected IDomainEventHandlerMappingStrategy MappingStrategy { get; set; }

        private void InitializeHandlers(object aggregateRootInstance)
        {
            foreach (var handler in MappingStrategy.GetEventHandlersFromAggregateRoot(aggregateRootInstance))
            {
                RegisterHandler(handler);
            }
        }

        public override void Initialize(object aggregateRootInstance)
        {
            base.Initialize(aggregateRootInstance);
            InitializeHandlers(aggregateRootInstance);
        }
    }
}