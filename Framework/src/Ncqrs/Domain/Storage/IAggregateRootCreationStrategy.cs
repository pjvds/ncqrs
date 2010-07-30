using System;

namespace Ncqrs.Domain.Storage
{
    public interface IAggregateRootCreationStrategy
    {
        AggregateRoot CreateAggregateRoot(Type aggregateRootType);
        T CreateAggregateRoot<T>() where T : AggregateRoot;
    }
}
