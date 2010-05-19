using System;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Domain
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWorkContext CreateUnitOfWork()
        {
            var store = NcqrsEnvironment.Get<IEventStore>();
            var bus = NcqrsEnvironment.Get<IEventBus>();

            var repository = new DomainRepository(store, bus);
            return new UnitOfWork(repository);
        }
    }
}
