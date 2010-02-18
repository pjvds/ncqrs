using System;
namespace Ncqrs.Domain.Storage
{
    public interface IDomainRepository
    {
        AggregateRoot GetById(Type aggregateRootType, Guid id);

        T GetById<T>(Guid id) where T : AggregateRoot;

        void Save(AggregateRoot t);
    }
}