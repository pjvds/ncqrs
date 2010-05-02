using System;
using Ncqrs.Domain.Storage;

namespace Ncqrs.Domain
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWorkContext CreateUnitOfWork()
        {
            var repository = NcqrsEnvironment.Get<IDomainRepository>();
            return new UnitOfWork(repository);
        }
    }
}
