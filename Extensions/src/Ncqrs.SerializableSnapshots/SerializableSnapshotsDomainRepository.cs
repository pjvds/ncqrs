using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.SerializableSnapshots
{
    public class SerializableSnapshotsDomainRepository : IDomainRepository
    {
        private const int SnapshotIntervalInEvents = 15;

        private readonly IEventBus _eventBus;
        private readonly IEventStore _store;
        private readonly ISerializableSnapshotStore _snapshotStore;
        private readonly IAggregateRootCreationStrategy _aggregateCreator;

        public SerializableSnapshotsDomainRepository(IEventStore store, IEventBus eventBus, ISerializableSnapshotStore snapshotStore, IAggregateRootCreationStrategy aggregateCreator = null)
        {
            Contract.Requires<ArgumentNullException>(store != null);
            Contract.Requires<ArgumentNullException>(eventBus != null);
            Contract.Requires<ArgumentNullException>(snapshotStore != null);

            _store = store;
            _eventBus = eventBus;
            _snapshotStore = snapshotStore;
            _aggregateCreator = aggregateCreator ?? new SimpleAggregateRootCreationStrategy();
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

        private AggregateRoot CreateEmptyAggRoot(Type aggType)
        {
            return _aggregateCreator.CreateAggregateRoot(aggType);
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
