using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing.Bus;
using Ncqrs.Eventing.Storage;

namespace Ncqrs.Domain
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
