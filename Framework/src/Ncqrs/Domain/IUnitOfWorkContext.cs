using System;

namespace Ncqrs.Domain
{
    public interface IUnitOfWorkContext : IDisposable
    {
        TAggregateRoot GetById<TAggregateRoot>(Guid id) where TAggregateRoot : AggregateRoot;

        AggregateRoot GetById(Type aggregateRootType, Guid id);

        void Accept();
    }
}