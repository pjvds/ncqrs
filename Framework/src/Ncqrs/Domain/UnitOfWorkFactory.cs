using System;
using Ncqrs.Commanding;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Domain
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWorkContext CreateUnitOfWork(Guid commandId)
        {
            if(UnitOfWorkContext.Current != null)
            {
                throw new InvalidOperationException("There is already a unit of work created for this context.");
            }

            var store = NcqrsEnvironment.Get<IEventStore>();
            var bus = NcqrsEnvironment.Get<IEventBus>();
            var snapshotStore = NcqrsEnvironment.Get<ISnapshotStore>();
            var snapshottingPolicy = NcqrsEnvironment.Get<ISnapshottingPolicy>();
            var aggregateCreationStrategy = NcqrsEnvironment.Get<IAggregateRootCreationStrategy>();
            var aggregateSnappshotter = NcqrsEnvironment.Get<IAggregateSnapshotter>();

            var repository = new DomainRepository(aggregateCreationStrategy, aggregateSnappshotter);
            var unitOfWork = new UnitOfWork(commandId, repository, store, snapshotStore, bus, snapshottingPolicy);
            UnitOfWorkContext.Bind(unitOfWork);
            return unitOfWork;
        }
    }
}
