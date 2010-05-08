using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Conversion;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;
using System.Collections.Generic;

namespace Ncqrs.Domain.Storage
{
    public class DomainRepository : IDomainRepository
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _store;
        private readonly IAggregateRootLoader _loader;
        private readonly IEventConverter _converter;

        public DomainRepository(IEventStore store, IEventBus eventBus)
            : this(store, eventBus, null, new DefaultAggregateRootLoader())
        {
            Contract.Requires<ArgumentNullException>(store != null, "store cannot be null.");
            Contract.Requires<ArgumentNullException>(eventBus != null, "store cannot be null.");
        }

        public DomainRepository(IEventStore store, IEventBus eventBus, IEventConverter converter, IAggregateRootLoader loader)
        {
            Contract.Requires<ArgumentNullException>(store != null);
            Contract.Requires<ArgumentNullException>(eventBus != null);
            Contract.Requires<ArgumentNullException>(loader != null);

            _store = store;
            _eventBus = eventBus;
            _loader = loader;
            _converter = converter;
        }

        public AggregateRoot GetById(Type aggregateRootType, Guid id)
        {
            var events = _store.GetAllEventsForEventSource(id).Cast<DomainEvent>();

            events = ConvertEvents(events);

            AggregateRoot aggregate = _loader.LoadAggregateRootFromEvents(aggregateRootType, events);
            return aggregate;
        }

        protected IEnumerable<DomainEvent> ConvertEvents(IEnumerable<DomainEvent> events)
        {
            if (_converter == null) return events;

            var result = new List<DomainEvent>(events.Count());

            foreach(var evnt in events)
            {
                var convertedEvent = (DomainEvent)_converter.Convert(evnt);
                result.Add(convertedEvent);
            }

            return result;
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
            // TODO: Remove eventbus, the repository should only push to store.
            _eventBus.Publish(events.Cast<IEvent>());

            // Accept the changes.
            aggregateRoot.AcceptChanges();
        }
    }
}
