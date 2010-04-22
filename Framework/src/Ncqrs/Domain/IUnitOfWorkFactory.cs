using System;

namespace Ncqrs.Domain
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork();

        IUnitOfWork GetUnitOfWorkInCurrentContext();
    }
}
