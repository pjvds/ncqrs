using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Domain.Storage
{
    public class DefaultAggregateSnapshotter : IAggregateSnapshotter
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IAggregateRootCreationStrategy _aggregateRootCreator;

        private readonly IAggregateSupportsSnapshotValidator _snapshotValidator;

        public DefaultAggregateSnapshotter(IAggregateRootCreationStrategy aggregateRootCreationStrategy, IAggregateSupportsSnapshotValidator snapshotValidator)
        {
            _aggregateRootCreator = aggregateRootCreationStrategy;
            _snapshotValidator = snapshotValidator;
        }

        public bool TryLoadFromSnapshot(Type aggregateRootType, Snapshot snapshot, CommittedEventStream committedEventStream, out AggregateRoot aggregateRoot)
        {
            aggregateRoot = null;

            if (snapshot == null) return false;

            if (AggregateSupportsSnapshot(aggregateRootType, snapshot.Payload.GetType()))
            {
                Log.DebugFormat("Reconstructing aggregate root {0}[{1}] from snapshot", aggregateRootType.FullName,
                                snapshot.EventSourceId.ToString("D"));
                aggregateRoot = _aggregateRootCreator.CreateAggregateRoot(aggregateRootType);
                aggregateRoot.InitializeFromSnapshot(snapshot);

                var memType = aggregateRoot.GetType().GetSnapshotInterfaceType();
                var restoreMethod = memType.GetMethod("RestoreFromSnapshot");

                restoreMethod.Invoke(aggregateRoot, new[] { snapshot.Payload });

                Log.DebugFormat("Applying remaining historic event to reconstructed aggregate root {0}[{1}]",
                    aggregateRootType.FullName, snapshot.EventSourceId.ToString("D"));
                aggregateRoot.InitializeFromHistory(committedEventStream);

                return true;
            }

            return false;
        }

        public bool TryTakeSnapshot(AggregateRoot aggregateRoot, out Snapshot snapshot)
        {
            snapshot = null;
            var memType = aggregateRoot.GetType().GetSnapshotInterfaceType();
            if (memType != null)
            {
                var createMethod = memType.GetMethod("CreateSnapshot");
                var payload = createMethod.Invoke(aggregateRoot, new object[0]);
                snapshot = new Snapshot(aggregateRoot.EventSourceId, aggregateRoot.Version, payload);
                return true;
            }
            return false;
        }

        private bool AggregateSupportsSnapshot(Type aggregateRootType, Type snapshotType)
        {
            return _snapshotValidator.DoesAggregateSupportsSnapshot(aggregateRootType, snapshotType);
        }

    }
    
}
