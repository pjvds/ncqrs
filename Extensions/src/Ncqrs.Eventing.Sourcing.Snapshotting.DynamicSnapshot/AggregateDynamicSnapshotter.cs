using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Storage;
using System.Reflection;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot
{
    public class AggregateDynamicSnapshotter : IAggregateSnapshotter
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IAggregateRootCreationStrategy _aggregateRootCreator;

        private readonly IAggregateSupportsSnapshotValidator _snapshotValidator;

        public AggregateDynamicSnapshotter(IAggregateRootCreationStrategy aggregateRootCreationStrategy, IAggregateSupportsSnapshotValidator snapshotValidator)
        {
            _aggregateRootCreator = aggregateRootCreationStrategy;
            _snapshotValidator = snapshotValidator;

            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                if (eventArgs.Name.Contains("DynamicSnapshot"))
                    return Assembly.LoadFrom("DynamicSnapshot.dll");
                return null;
            };
        }

        public bool TryLoadFromSnapshot(Type aggregateRootType, Snapshot snapshot, CommittedEventStream committedEventStream, out Domain.AggregateRoot aggregateRoot)
        {
            aggregateRoot = null;

            if (snapshot == null) return false;

            if (AggregateSupportsSnapshot(aggregateRootType, snapshot.Payload.GetType()))
            {
                try
                {
                    Log.DebugFormat("Reconstructing aggregate root {0}[{1}] from snapshot", aggregateRootType.FullName,
                                    snapshot.EventSourceId.ToString("D"));
                    aggregateRoot = _aggregateRootCreator.CreateAggregateRoot(aggregateRootType);
                    aggregateRoot.InitializeFromSnapshot(snapshot);
                    aggregateRoot.RestoreFromSnapshot(snapshot.Payload);

                    Log.DebugFormat("Applying remaining historic event to reconstructed aggregate root {0}[{1}]",
                        aggregateRootType.FullName, snapshot.EventSourceId.ToString("D"));
                    aggregateRoot.InitializeFromHistory(committedEventStream);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Cannot load snapshot for '{0}' aggregate. {1}",
                        aggregateRoot.GetType().FullName, ex.Message);
                    aggregateRoot = null;
                    return false;
                }

                return true;
            }

            return false;
        }

        public bool TryTakeSnapshot(Domain.AggregateRoot aggregateRoot, out Snapshot snapshot)
        {
            snapshot = null;
            try
            {
                var payload = ((dynamic)aggregateRoot).CreateSnapshot();
                snapshot = new Snapshot(aggregateRoot.EventSourceId, aggregateRoot.Version, payload);
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Cannot take snapshot for '{0}' aggregate. {1}",
                        aggregateRoot.GetType().FullName, ex.Message);
                snapshot = null;
                return false;
            }
        }

        private bool AggregateSupportsSnapshot(Type aggregateRootType, Type snapshotType)
        {
            return _snapshotValidator.DoesAggregateSupportsSnapshot(aggregateRootType, snapshotType);
        }
    }
}
