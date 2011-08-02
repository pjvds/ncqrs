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

        private readonly IAggregateRootCreationStrategy _aggregateRootCreator;

        private readonly IAggregateSnapshotter _aggregateSnapshotter;

        public DomainRepository(IAggregateRootCreationStrategy aggregateRootCreationStrategy, IAggregateSnapshotter aggregateSnapshotter)
        {
            _aggregateRootCreator = aggregateRootCreationStrategy;
            _aggregateSnapshotter = aggregateSnapshotter;
        }

        public AggregateRoot Load(Type aggreateRootType, Snapshot snapshot, CommittedEventStream eventStream)
        {
            AggregateRoot aggregate = null;

            if (!_aggregateSnapshotter.TryLoadFromSnapshot(aggreateRootType, snapshot, eventStream, out aggregate))
                aggregate = GetByIdFromScratch(aggreateRootType, eventStream);

            return aggregate;
        }

        public Snapshot TryTakeSnapshot(AggregateRoot aggregateRoot)
        {
            Snapshot snapshot = null;
            _aggregateSnapshotter.TryTakeSnapshot(aggregateRoot, out snapshot);
            return snapshot;
        }

        protected AggregateRoot GetByIdFromScratch(Type aggregateRootType, CommittedEventStream committedEventStream)
        {
            AggregateRoot aggregateRoot = null;
            Log.DebugFormat("Reconstructing aggregate root {0}[{1}] directly from event stream", aggregateRootType.FullName,
                               committedEventStream.SourceId.ToString("D"));

            if (committedEventStream.Count() > 0)
            {
                aggregateRoot = _aggregateRootCreator.CreateAggregateRoot(aggregateRootType);
                aggregateRoot.InitializeFromHistory(committedEventStream);
            }

            return aggregateRoot;
        }

    }
}
