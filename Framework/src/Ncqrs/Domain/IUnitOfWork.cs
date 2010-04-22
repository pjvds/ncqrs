using System;
using Ncqrs.Domain.Storage;

namespace Ncqrs.Domain
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
