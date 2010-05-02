using System;
using Ncqrs.Domain.Storage;

namespace Ncqrs.Domain
{
    public interface IUnitOfWorkContext : IDisposable
    {
        IDomainRepository Repository
        {
            get;
        }

        void Accept();
    }
}
