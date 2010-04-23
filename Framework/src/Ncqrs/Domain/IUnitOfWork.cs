using System;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        IDomainRepository Repository
        {
            get;
        }

        void Accept();

        void RegisterDirtyInstance(AggregateRoot dirtyAggregateRoot);
    }
}
