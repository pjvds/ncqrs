using System;
using Ncqrs.Commanding;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Domain
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWorkContext CreateUnitOfWork(Guid commandId)
        {
            if(UnitOfWork.Current != null) throw new InvalidOperationException("There is already a unit of work created for this context.");

            var store = NcqrsEnvironment.Get<IEventStore>();
            var bus = NcqrsEnvironment.Get<IEventBus>();
            var snapshotStore = NcqrsEnvironment.Get<ISnapshotStore>();

            var repository = new DomainRepository(store, bus, snapshotStore);
            return new UnitOfWork(commandId, repository);
        }
    }
}
