using System;

namespace Ncqrs.Domain.Storage
{
    /// <summary>
    /// Create AggregateRoots for the domain repository, it will then replay events into them
    /// to re-construct their current state.
    /// </summary>
    public interface IAggregateRootCreationStrategy
    {
        AggregateRoot CreateAggregateRoot(Type aggregateRootType);
        T CreateAggregateRoot<T>() where T : AggregateRoot;
    }
}
