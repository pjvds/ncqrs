using System;
using System.Linq.Expressions;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    public interface IUnitOfWorkContext : IDisposable
    {
        TAggregateRoot Create<TAggregateRoot>(params object[] constructorArguments) where TAggregateRoot : IAggregateRoot;
        TAggregateRoot GetById<TAggregateRoot>(Guid id) where TAggregateRoot : IAggregateRoot;
        IAggregateRoot GetById(Type aggregateRootType, Guid id);

        void Accept();
    }
}
