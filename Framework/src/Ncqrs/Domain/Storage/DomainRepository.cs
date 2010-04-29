using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Domain.Storage
{
    public class DomainRepository : IDomainRepository
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _store;
        private readonly IAggregateRootLoader _loader;

        public DomainRepository(IEventStore store, IEventBus eventBus) : this(store, eventBus, new DefaultAggregateRootLoader())
        {
            Contract.Requires<ArgumentNullException>(store != null, "store cannot be null.");
            Contract.Requires<ArgumentNullException>(eventBus != null, "store cannot be null.");
        }

        public DomainRepository(IEventStore store, IEventBus eventBus, IAggregateRootLoader loader)
        {
            Contract.Requires<ArgumentNullException>(store != null);
            Contract.Requires<ArgumentNullException>(eventBus != null);
            Contract.Requires<ArgumentNullException>(loader != null);

            _store = store;
            _eventBus = eventBus;
            _loader = loader;
        }

        public AggregateRoot GetById(Type aggregateRootType, Guid id)
        {
            var events = _store.GetAllEventsForEventSource(id).Cast<DomainEvent>();
            AggregateRoot aggregate = null;

            try
            {
                aggregate = _loader.LoadAggregateRootFromEvents(aggregateRootType, events);
            }
            catch (MissingMethodException)
            {
                // TODO: Retrow exception with better details that there is no public ctor found that takes a IEnumerable<HistoricalEvent>.
                throw;
            }

            return aggregate;
        }

        public T GetById<T>(Guid id) where T : AggregateRoot
        {
            return (T)GetById(typeof(T), id);
        }

        public void Save(AggregateRoot aggregateRoot)
        {
            var events = aggregateRoot.GetUncommittedEvents();

            // Save the events to the event store.
            _store.Save(aggregateRoot);

            // Send all events to the bus.
            // TODO: Remove cast co/con
            _eventBus.Publish(events.Cast<IEvent>());

            // Accept the changes.
            aggregateRoot.CommitEvents();
        }
    }
}
