using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Domain.Storage
{
    public class DomainRepository : IDomainRepository
    {
        //private const int SnapshotIntervalInEvents = 15;

        private readonly IEventBus _eventBus;
        private readonly IEventStore _store;
        private readonly ISnapshotStore _snapshotStore;
        private readonly IAggregateRootCreationStrategy _aggregateRootCreator = new SimpleAggregateRootCreationStrategy();

        public DomainRepository(IEventStore store, IEventBus eventBus, ISnapshotStore snapshotStore = null, IAggregateRootCreationStrategy aggregateRootCreationStrategy = null)
        {
            Contract.Requires<ArgumentNullException>(store != null);
            Contract.Requires<ArgumentNullException>(eventBus != null);

            _store = store;
            _eventBus = eventBus;
            _snapshotStore = snapshotStore;
            _aggregateRootCreator = aggregateRootCreationStrategy ?? new SimpleAggregateRootCreationStrategy();
        }

        //Creates a snapshot if the 
        //private bool ShouldCreateSnapshot(AggregateRoot aggregateRoot)
        //{
        //    if (_snapshotStore != null)
        //        for (var i = aggregateRoot.InitialVersion + 1; i <= aggregateRoot.Version; i++)
        //            if (i % SnapshotIntervalInEvents == 0) return true;
        //    return false;
        //}        


        /// <summary>
        /// Gets aggregate root by eventSourceId.
        /// </summary>
        /// <param name="aggregateRootType">Type of the aggregate root.</param>
        /// <param name="eventSourceId">The eventSourceId of the aggregate root.</param>
        /// <param name="lastKnownRevision">If specified, the most recent version of event source 
        /// observed by the client (used for optimistic concurrency).</param>
        /// <returns>A new instance of the aggregate root that contains the latest known state or null if aggregate does not exist.</returns>
        public AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId, long? lastKnownRevision)
        {
            AggregateRoot aggregateRoot = null;
            if (_snapshotStore != null)
            {
                var snapshot = _snapshotStore.GetSnapshot(eventSourceId);

                if (snapshot != null)
                {
                    aggregateRoot = GetByIdFromSnapshot(aggregateRootType, snapshot);
                }
            }

            if (aggregateRoot == null)
            {
                aggregateRoot = GetByIdFromScratch(aggregateRootType, eventSourceId, lastKnownRevision);
            }
            return aggregateRoot;
        }

        protected AggregateRoot GetByIdFromSnapshot(Type aggregateRootType, ISnapshot snapshot)
        {
            AggregateRoot aggregateRoot;

            if(AggregateRootSupportsSnapshot(aggregateRootType, snapshot))
            {
                aggregateRoot = CreateEmptyAggRoot(aggregateRootType);
                var memType = GetSnapshotInterfaceType(aggregateRootType);
                var restoreMethod = memType.GetMethod("RestoreFromSnapshot");

                restoreMethod.Invoke(aggregateRoot, new object[] { snapshot });

                var nextId = snapshot.EventSourceVersion + 1;
                var events = _store.ReadFrom(aggregateRoot.EventSourceId, nextId);
                aggregateRoot.InitializeFromHistory(events);
            }
            else
            {
                aggregateRoot = GetByIdFromScratch(aggregateRootType, snapshot.EventSourceId, null);
            }

            return aggregateRoot;
        }

        protected AggregateRoot GetByIdFromScratch(Type aggregateRootType, Guid id, long ? lastKnownRevision)
        {
            AggregateRoot aggregateRoot = null;

            var events = _store.ReadUntil(id, lastKnownRevision);

            if (events.Count() > 0)
            {
                aggregateRoot = CreateEmptyAggRoot(aggregateRootType);
                aggregateRoot.InitializeFromHistory(events);
            }

            return aggregateRoot;
        }

        protected AggregateRoot CreateEmptyAggRoot(Type aggregateRootType)
        {
            return _aggregateRootCreator.CreateAggregateRoot(aggregateRootType);
        }

        private bool AggregateRootSupportsSnapshot(Type aggType, ISnapshot snapshot)
        {
            var memType = GetSnapshotInterfaceType(aggType);
            var snapshotType = snapshot.GetType();

            var expectedType = typeof (ISnapshotable<>).MakeGenericType(snapshotType);
            return memType == expectedType;
        }

        public void Store(UncommittedEventStream eventStream)
        {
            _store.Store(eventStream);
            _eventBus.Publish(eventStream);
        }

        //private ISnapshot GetSnapshot(AggregateRoot aggregateRoot)
        //{
        //    var memType = GetSnapshotInterfaceType(aggregateRoot.GetType());

        //    if (memType != null)
        //    {
        //        var createMethod = memType.GetMethod("CreateSnapshot");
        //        return (ISnapshot)createMethod.Invoke(aggregateRoot, new object[0]);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        private Type GetSnapshotInterfaceType(Type aggType)
        {
            // Query all ISnapshotable interfaces. We only allow only
            // one ISnapshotable interface per aggregate root type.
            var snapshotables = from i in aggType.GetInterfaces()
                                where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISnapshotable<>)
                                select i;

            // Aggregate does not implement any ISnapshotable interface.
            if (snapshotables.Count() == 0)
            {
                return null;
            }
            // Aggregate does implement multiple ISnapshotable interfaces.
            if (snapshotables.Count() > 1)
            {
                return null;
            }

            return snapshotables.Single();
        }
    }
}
