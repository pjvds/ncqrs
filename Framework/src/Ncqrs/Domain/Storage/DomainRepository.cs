using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            var events = _store.GetAllEventsForEventSource(id);
            var eventsAsDomainEvents = new List<DomainEvent>();

            // TODO: Is there a better way to cast?
            foreach (var evnt in events)
            {
                eventsAsDomainEvents.Add((DomainEvent)evnt);
            }

            AggregateRoot aggregate = null;

            try
            {
                aggregate = _loader.LoadAggregateRootFromEvents(aggregateRootType, eventsAsDomainEvents);
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
            // Save the events to the event store.
            IEnumerable<IEvent> events = _store.Save(aggregateRoot);

            // Send all events to the bus.
            _eventBus.Publish(events);

            // Accept the changes.
            aggregateRoot.CommitEvents();
        }
    }
}
