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
        private const int SnapshotIntervalInEvents = 15;

        private readonly IEventBus _eventBus;
        private readonly IEventStore _store;
        private readonly ISnapshotStore _snapshotStore;
        private readonly IAggregateRootLoader _loader;
        private readonly IEventConverter<DomainEvent, DomainEvent> _converter;

        public DomainRepository(IEventStore store, IEventBus eventBus)
            : this(store, eventBus, new DefaultAggregateRootLoader())
        {
            Contract.Requires<ArgumentNullException>(store != null, "store cannot be null.");
            Contract.Requires<ArgumentNullException>(eventBus != null, "store cannot be null.");
        }

        public DomainRepository(IEventStore store, IEventBus eventBus, IAggregateRootLoader loader, ISnapshotStore snapshotStore = null, IEventConverter<DomainEvent, DomainEvent> converter = null)
        {
            Contract.Requires<ArgumentNullException>(store != null);
            Contract.Requires<ArgumentNullException>(eventBus != null);
            Contract.Requires<ArgumentNullException>(loader != null);

            _store = store;
            _eventBus = eventBus;
            _loader = loader;
            _converter = converter;
        }

        private bool ShouldCreateSnapshot(AggregateRoot aggregateRoot)
        {
            return (_snapshotStore != null )&&(aggregateRoot.Version % SnapshotIntervalInEvents) == 0;
        }

        public AggregateRoot GetById(Type aggregateRootType, Guid id)
        {
            var events = _store.GetAllEvents(id).Cast<DomainEvent>();

            events = ConvertEvents(events);

            AggregateRoot aggregate = _loader.LoadAggregateRootFromEvents(aggregateRootType, events);
            return aggregate;
        }

        protected IEnumerable<DomainEvent> ConvertEvents(IEnumerable<DomainEvent> events)
        {
            if (_converter == null) return events;

            var result = new List<DomainEvent>(events.Count());

            foreach (var evnt in events)
            {
                var convertedEvent = _converter.Convert(evnt);
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

            _store.Save(aggregateRoot);

            if(ShouldCreateSnapshot(aggregateRoot))
            {
                var snapshot = GetSnapshot(aggregateRoot);

                if(snapshot != null) _snapshotStore.SaveShapshot(snapshot);
            }

            _eventBus.Publish(events.Cast<IEvent>());

            // Accept the changes.
            aggregateRoot.AcceptChanges();
        }

        private ISnapshot GetSnapshot(AggregateRoot aggregateRoot)
        {
            Type aggType = aggregateRoot.GetType();


            // Query all IMementoable interfaces. We only allow
            // one IMementoable interface per aggregate root type.
            var mementoables = from i in aggType.GetInterfaces()
                               where i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IMementoable<>)
                               select i;

            // Aggregate does not implement any IMementoable interface.
            if (mementoables.Count() == 0)
            {
                return null;
            }
            // Aggregate does implement multiple IMementoable interfaces.
            if (mementoables.Count() > 0)
            {
                return null;
            }

            var mementoable = mementoables.First();
            var createMethod = mementoable.GetMethod("CreateMemento");

            IMemento memento = (IMemento) createMethod.Invoke(aggregateRoot, new object[0]);
            return new Snapshot(aggregateRoot.Id, aggregateRoot.Version, memento);
        }
    }
}
