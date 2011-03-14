using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Domain.Storage
{
    public class DomainRepository : IDomainRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IAggregateRootCreationStrategy _aggregateRootCreator = new SimpleAggregateRootCreationStrategy();

        public DomainRepository(IAggregateRootCreationStrategy aggregateRootCreationStrategy = null)
        {
            _aggregateRootCreator = aggregateRootCreationStrategy ?? new SimpleAggregateRootCreationStrategy();
        }

        public AggregateRoot Load(Type aggreateRootType, Snapshot snapshot, CommittedEventStream eventStream)
        {
            return snapshot != null 
                ? GetByIdFromSnapshot(aggreateRootType, snapshot, eventStream) 
                : GetByIdFromScratch(aggreateRootType, eventStream);
        }

        protected AggregateRoot GetByIdFromSnapshot(Type aggregateRootType, Snapshot snapshot, CommittedEventStream committedEventStream)
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

                Log.DebugFormat("Appying remaining historic event to reconstructed aggregate root {0}[{1}]",
                    aggregateRootType.FullName, snapshot.EventSourceId.ToString("D"));
                aggregateRoot.InitializeFromHistory(committedEventStream);
            }
            else
            {
                Log.WarnFormat(
                    "Found snapshot for aggregate root {0}[{1}], but aggregate root class does not support snapshots",
                    aggregateRootType.FullName, snapshot.EventSourceId.ToString("D"));
                aggregateRoot = GetByIdFromScratch(aggregateRootType, committedEventStream);
            }

            return aggregateRoot;
        }

        protected AggregateRoot GetByIdFromScratch(Type aggregateRootType, CommittedEventStream committedEventStream)
        {
            AggregateRoot aggregateRoot = null;
            Log.DebugFormat("Reconstructing agregate root {0}[{1}] directly from event stream", aggregateRootType.FullName,
                               committedEventStream.SourceId.ToString("D"));

            if (committedEventStream.Count() > 0)
            {
                aggregateRoot = CreateEmptyAggRoot(aggregateRootType);
                aggregateRoot.InitializeFromHistory(committedEventStream);
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

        public Snapshot TryTakeSnapshot(AggregateRoot aggregateRoot)
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
                Log.DebugFormat("No snapshot interface found on aggregate root {0}.", aggType.FullName);
                return null;
            }
            // Aggregate does implement multiple ISnapshotable interfaces.
            if (snapshotables.Count() > 1)
            {
                Log.WarnFormat("Aggregate root {0} contains multiple snapshot interfaces while only one is allowed.", aggType.FullName);
                return null;
            }

            var snapshotableInterfaceType = snapshotables.Single();
            Log.DebugFormat("Found snapshot interface {0} on aggregate root {1}.", snapshotableInterfaceType.FullName, aggType.FullName);

            return snapshotableInterfaceType;
        }
    }
}
