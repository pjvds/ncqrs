using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.SerializableSnapshots
{
    public class SerializableSnapshotsDomainRepository : IDomainRepository
    {
        private const int SnapshotIntervalInEvents = 15;

        private readonly IEventBus _eventBus;
        private readonly IEventStore _store;
        private readonly ISerializableSnapshotStore _snapshotStore;

        public SerializableSnapshotsDomainRepository(IEventStore store, IEventBus eventBus, ISerializableSnapshotStore snapshotStore)
        {
            Contract.Requires<ArgumentNullException>(store != null);
            Contract.Requires<ArgumentNullException>(eventBus != null);
            Contract.Requires<ArgumentNullException>(snapshotStore != null);

            _store = store;
            _eventBus = eventBus;
            _snapshotStore = snapshotStore;
        }

        private static bool ShouldCreateSnapshot(AggregateRoot aggregateRoot)
        {
            return aggregateRoot.Version % SnapshotIntervalInEvents == 0;
        }

        public AggregateRoot GetById(Type aggregateRootType, Guid id)
        {
            var aggregate = (AggregateRoot) _snapshotStore.GetSnapshot(id);

            if (aggregate != null)
            {
                GetByIdFromSnapshot(aggregate);
            }
            else
            {
                aggregate = GetByIdFromScratch(aggregateRootType, id);
            }

            return aggregate;
        }

        protected AggregateRoot GetByIdFromSnapshot(AggregateRoot aggregateRoot)
        {
            var events = _store.GetAllEventsSinceVersion(aggregateRoot.EventSourceId, aggregateRoot.InitialVersion);
            aggregateRoot.InitializeFromHistory(events);

            return aggregateRoot;
        }

        protected AggregateRoot GetByIdFromScratch(Type aggregateRootType, Guid id)
        {
            AggregateRoot aggregateRoot = null;

            var events = _store.GetAllEvents(id);

            if (events.Count() > 0)
            {
                aggregateRoot = CreateEmptyAggRoot(aggregateRootType);
                aggregateRoot.InitializeFromHistory(events);
            }

            return aggregateRoot;
        }

        private static AggregateRoot CreateEmptyAggRoot(Type aggType)
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
            var aggregateRoot = (AggregateRoot)ctor.Invoke(null);

            return aggregateRoot;
        }

        public T GetById<T>(Guid id) where T : AggregateRoot
        {
            return (T)GetById(typeof(T), id);
        }

        public void Save(AggregateRoot aggregateRoot)
        {
            var events = aggregateRoot.GetUncommittedEvents();

            _store.Save(aggregateRoot);

            if (ShouldCreateSnapshot(aggregateRoot))
            {
                _snapshotStore.SaveSnapshot(aggregateRoot);
            }

            _eventBus.Publish(events.Cast<IEvent>());

            // Accept the changes.
            aggregateRoot.AcceptChanges();
        }                
    }
}
