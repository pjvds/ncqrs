using System;
using EventStore;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JoesUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IStoreEvents _eventStore;

        public JoesUnitOfWorkFactory(IStoreEvents eventStore)
        {
            _eventStore = eventStore;
        }

        public IUnitOfWorkContext CreateUnitOfWork(Guid commandId)
        {
            if (UnitOfWorkContext.Current != null)
            {
                throw new InvalidOperationException("There is already a unit of work created for this context.");
            }

            var store = NcqrsEnvironment.Get<IEventStore>();
            var bus = NcqrsEnvironment.Get<IEventBus>();
            var snapshotStore = NcqrsEnvironment.Get<ISnapshotStore>();
            var snapshottingPolicy = NcqrsEnvironment.Get<ISnapshottingPolicy>();
            var aggregateCreationStrategy = NcqrsEnvironment.Get<IAggregateRootCreationStrategy>();
            var aggregateSupportsSnapshotValidator = NcqrsEnvironment.Get<IAggregateSupportsSnapshotValidator>();
            var aggregateSnappshotter = NcqrsEnvironment.Get<IAggregateSnapshotter>();

            var repository = new DomainRepository(aggregateCreationStrategy, aggregateSnappshotter);
            var unitOfWork = new JoesUnitOfWork(commandId, repository, _eventStore, snapshotStore, bus, snapshottingPolicy);
            UnitOfWorkContext.Bind(unitOfWork);
            return unitOfWork;
        }
    }
}