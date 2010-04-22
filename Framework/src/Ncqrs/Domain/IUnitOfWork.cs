using System;
using Ncqrs.Eventing.ServiceModel.Bus.Storage;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public interface IUnitOfWork : IDisposable
    {
        IDomainRepository Repository
        {
            get;
        }

        void Accept();
    }
}
