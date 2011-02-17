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
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int SnapshotIntervalInEvents = 2;

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
            _snapshotStore = snapshotStore ?? new NullSnapshotStore();
            _aggregateRootCreator = aggregateRootCreationStrategy ?? new SimpleAggregateRootCreationStrategy();
        }

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
            Log.DebugFormat("Retrieving aggregate root of {0}[{1}] (revision: {0}) from the repository",
                aggregateRootType.FullName, eventSourceId.ToString("D"), lastKnownRevision.HasValue ? lastKnownRevision.ToString() : "latest");
            if (!lastKnownRevision.HasValue)
            {
                Log.WarnFormat("Retrieving aggregate root of {0}[{1}] withous specifying revision. No optimistic concurrency check will be done",
                  aggregateRootType.FullName, eventSourceId.ToString("D"));  
            }
            AggregateRoot aggregateRoot = null;
            long maxVersion = lastKnownRevision.HasValue ? lastKnownRevision.Value : long.MaxValue;
            if (_snapshotStore != null)
            {
                var snapshot = _snapshotStore.GetSnapshot(eventSourceId, maxVersion);

                if (snapshot != null)
                {
                    aggregateRoot = GetByIdFromSnapshot(aggregateRootType, snapshot, maxVersion);
                }
            }

            if (aggregateRoot == null)
            {
                aggregateRoot = GetByIdFromScratch(aggregateRootType, eventSourceId, maxVersion);
            }
            return aggregateRoot;
        }

        protected AggregateRoot GetByIdFromSnapshot(Type aggregateRootType, Snapshot snapshot, long lastKnownRevision)
        {
            AggregateRoot aggregateRoot;

            if(AggregateRootSupportsSnapshot(aggregateRootType, snapshot))
            {
                Log.DebugFormat("Reconstructing agregate root {0}[{1}] from snapshot", aggregateRootType.FullName,
                                snapshot.EventSourceId.ToString("D"));
                aggregateRoot = CreateEmptyAggRoot(aggregateRootType);
                aggregateRoot.InitializeFromSnapshot(snapshot);
                var memType = GetSnapshotInterfaceType(aggregateRootType);
                var restoreMethod = memType.GetMethod("RestoreFromSnapshot");

                restoreMethod.Invoke(aggregateRoot, new[] { snapshot.Payload });

                var nextId = snapshot.Version + 1;
                var events = _store.ReadFrom(aggregateRoot.EventSourceId, nextId, int.MaxValue);
                Log.DebugFormat("Appying remaining historic event to reconstructed aggregate root {0}[{1}]",
                    aggregateRootType.FullName, snapshot.EventSourceId.ToString("D"));
                aggregateRoot.InitializeFromHistory(events);
            }
            else
            {
                Log.WarnFormat(
                    "Found snapshot for aggregate root {0}[{1}], but aggregate root class does not support snapshots",
                    aggregateRootType.FullName, snapshot.EventSourceId.ToString("D"));
                aggregateRoot = GetByIdFromScratch(aggregateRootType, snapshot.EventSourceId, lastKnownRevision);
            }

            return aggregateRoot;
        }

        protected AggregateRoot GetByIdFromScratch(Type aggregateRootType, Guid id, long lastKnownRevision)
        {
            AggregateRoot aggregateRoot = null;
            Log.DebugFormat("Reconstructing agregate root {0}[{1}] directly from event stream", aggregateRootType.FullName,
                               id.ToString("D"));
            var events = _store.ReadFrom(id, long.MinValue, lastKnownRevision);

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

        private static bool AggregateRootSupportsSnapshot(Type aggType, Snapshot snapshot)
        {
            var memType = GetSnapshotInterfaceType(aggType);
            var snapshotType = snapshot.Payload.GetType();

            var expectedType = typeof (ISnapshotable<>).MakeGenericType(snapshotType);
            return memType == expectedType;
        }

        public void Store(UncommittedEventStream eventStream)
        {
            Log.DebugFormat("Storing the event stream for command {0} to event store", eventStream.CommitId);
            _store.Store(eventStream);
            Log.DebugFormat("Publishing events for command {0} to event bus", eventStream.CommitId);
            _eventBus.Publish(eventStream);
        }

        public void CreateSnapshotIfNecessary(AggregateRoot aggregateRoot)
        {
            if (ShouldCreateSnapshot(aggregateRoot))
            {
                var snapshot = GetSnapshot(aggregateRoot);
                if (snapshot != null)
                {
                    _snapshotStore.SaveShapshot(snapshot);
                }
            }
        }

        private static bool ShouldCreateSnapshot(AggregateRoot aggregateRoot)
        {
            for (var i = aggregateRoot.InitialVersion + 1; i <= aggregateRoot.Version; i++)
            {
                if (i%SnapshotIntervalInEvents == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static Snapshot GetSnapshot(AggregateRoot aggregateRoot)
        {
            var memType = GetSnapshotInterfaceType(aggregateRoot.GetType());
            if (memType != null)
            {
                var createMethod = memType.GetMethod("CreateSnapshot");
                var payload = createMethod.Invoke(aggregateRoot, new object[0]);
                return new Snapshot(aggregateRoot.EventSourceId, aggregateRoot.Version, payload);
            }
            return null;
        }

        private static Type GetSnapshotInterfaceType(Type aggType)
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
