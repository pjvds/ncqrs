using System;
using Ncqrs.Eventing.ServiceModel.Bus.Storage;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public class ThreadBasedUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private IDomainRepository _repository;

        public ThreadBasedUnitOfWorkFactory(IEventStore eventStore, IEventBus eventBus)
        {
            _repository = new DomainRepository(eventStore, eventBus);
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(_repository);
        }

        public IUnitOfWork GetUnitOfWorkInCurrentContext()
        {
            return UnitOfWork.Current;
        }
    }
}
