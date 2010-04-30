using System;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing.ServiceModel.Bus;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Domain
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private IDomainRepository _repository;

        public UnitOfWorkFactory(IEventStore eventStore, IEventBus eventBus)
        {
            _repository = new DomainRepository(eventStore, eventBus);
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(_repository);
        }
    }
}
