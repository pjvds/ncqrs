using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
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
        private readonly IEventConverter<DomainEvent, DomainEvent> _converter;

        public DomainRepository(IEventStore store, IEventBus eventBus, ISnapshotStore snapshotStore = null, IEventConverter<DomainEvent, DomainEvent> converter = null)
        {
            Contract.Requires<ArgumentNullException>(store != null);
            Contract.Requires<ArgumentNullException>(eventBus != null);

            _store = store;
            _eventBus = eventBus;
            _converter = converter;
            _snapshotStore = snapshotStore;
        }

        private bool ShouldCreateSnapshot(AggregateRoot aggregateRoot)
        {
            return (_snapshotStore != null)&&(aggregateRoot.Version % SnapshotIntervalInEvents) == 0;
        }

        public AggregateRoot GetById(Type aggregateRootType, Guid id)
        {
            AggregateRoot aggregate = null;

            if(_snapshotStore != null)
            {
                var snapshot = _snapshotStore.GetSnapshot(id);

                if (snapshot != null)
                {
                    aggregate = GetByIdFromSnapshot(aggregateRootType, snapshot);
                }
            }

            if(aggregate == null)
            {
                aggregate = GetByIdFromScratch(aggregateRootType, id);
            }

            return aggregate;
        }

        protected AggregateRoot GetByIdFromSnapshot(Type aggregateRootType, ISnapshot snapshot)
        {
            AggregateRoot aggregateRoot = null;

            if(AggregateRootSupportsSnapshot(aggregateRootType, snapshot))
            {
                aggregateRoot = CreateEmptyAggRoot(aggregateRootType);
                var memType = GetMementoableInterfaceType(aggregateRootType);
                var restoreMethod = memType.GetMethod("RestoreFromSnapshot");

                restoreMethod.Invoke(aggregateRoot, new object[] { snapshot });

                var events = _store.GetAllEventsSinceVersion(aggregateRoot.Id, snapshot.EventSourceVersion);
                aggregateRoot.InitializeFromHistory(events.Cast<DomainEvent>());
            }
            else
            {
                aggregateRoot = GetByIdFromScratch(aggregateRootType, snapshot.EventSourceId);
            }

            return aggregateRoot;
        }

        protected AggregateRoot GetByIdFromScratch(Type aggregateRootType, Guid id)
        {
            AggregateRoot aggregateRoot = null;

            var events = _store.GetAllEvents(id).Cast<DomainEvent>();
            events = ConvertEvents(events);

            if (events.Count() > 0)
            {
                aggregateRoot = CreateEmptyAggRoot(aggregateRootType);
                aggregateRoot.InitializeFromHistory(events);
            }

            return aggregateRoot;
        }

        private bool AggregateRootSupportsSnapshot(Type aggType, ISnapshot snapshot)
        {
            var memType = GetMementoableInterfaceType(aggType);
            return memType == typeof(ISnapshotable<>).MakeGenericType(memType);
        }

        private AggregateRoot CreateEmptyAggRoot(Type aggType)
        {
            // Flags to search for a public and non public contructor.
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            // Get the constructor that we want to invoke.
            var ctor = aggType.GetConstructor(flags, null, Type.EmptyTypes, null);

            // If there was no ctor found, throw exception.
            if (ctor == null)
            {
                var message = String.Format("No constructor found on aggregate root type {0} that accepts " +
                                            "no parameters.", aggType.AssemblyQualifiedName);
                throw new AggregateLoaderException(message);
            }

            // There was a ctor found, so invoke it and return the instance.
            var aggregateRoot = (AggregateRoot) ctor.Invoke(null);

            return aggregateRoot;
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

            // TODO: Snapshot should not effect saving.
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
            var memType = GetMementoableInterfaceType(aggregateRoot.GetType());

            if (memType != null)
            {
                var createMethod = memType.GetMethod("CreateSnapshot");

                return (ISnapshot)createMethod.Invoke(aggregateRoot, new object[0]);
            }
            else
            {
                return null;
            }
        }

        private Type GetMementoableInterfaceType(Type aggType)
        {
            // Query all ISnapshotable interfaces. We only allow
            // one ISnapshotable interface per aggregate root type.
            var mementoables = from i in aggType.GetInterfaces()
                               where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISnapshotable<>)
                               select i;

            // Aggregate does not implement any ISnapshotable interface.
            if (mementoables.Count() == 0)
            {
                return null;
            }
            // Aggregate does implement multiple ISnapshotable interfaces.
            if (mementoables.Count() > 0)
            {
                return null;
            }

            return mementoables.First();
        }
    }
}
