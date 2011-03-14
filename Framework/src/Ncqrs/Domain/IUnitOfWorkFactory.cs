using System;
using Ncqrs.Commanding;

namespace Ncqrs.Domain
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWorkContext CreateUnitOfWork(Guid commandId);
    }
}
