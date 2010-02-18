using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Bus;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    public class DomainRepository : IDomainRepository
    {
        private readonly IEventBus _eventBus;
        private readonly IEventStore _store;

        public DomainRepository(IEventStore store, IEventBus eventBus)
        {
            _store = store;
            _eventBus = eventBus;
        }

        public AggregateRoot GetById(Type aggregateRootType, Guid id)
        {
            var events = _store.GetAllEventsForEventSource(id);
            AggregateRoot aggregate = null;

            try
            {
                aggregate = (AggregateRoot)Activator.CreateInstance(aggregateRootType, events);
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

        internal void Save(AggregateRoot aggregateRoot)
        {
            // Save the events to the event store.
            IEnumerable<IEvent> events = _store.Save(aggregateRoot);

            // Send all events to the bus.
            _eventBus.Publish(events);

            // Accept the changes.
            aggregateRoot.AcceptEvents();
        }

        AggregateRoot IDomainRepository.GetById(Type aggregateRootType, Guid id)
        {
            throw new NotImplementedException();
        }

        T IDomainRepository.GetById<T>(Guid id)
        {
            throw new NotImplementedException();
        }

        void IDomainRepository.Save(AggregateRoot t)
        {
            Save(t);
        }
    }
}
