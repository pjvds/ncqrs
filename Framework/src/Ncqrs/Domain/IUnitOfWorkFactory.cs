using System;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork();

        IUnitOfWork GetUnitOfWorkInCurrentContext();
    }
}
