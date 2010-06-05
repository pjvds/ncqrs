using System;
using Ncqrs.Domain;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.SerializableSnapshots
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWorkContext CreateUnitOfWork()
        {
            if (UnitOfWork.Current != null) throw new InvalidOperationException("There is already a unit of work created for this context.");

            var store = NcqrsEnvironment.Get<IEventStore>();
            var bus = NcqrsEnvironment.Get<IEventBus>();
            var snapshotStore = NcqrsEnvironment.Get<ISerializableSnapshotStore>();

            var repository = new SerializableSnapshotsDomainRepository(store, bus, snapshotStore);
            return new UnitOfWork(repository);
        }
    }
}