namespace Ncqrs.Config.Autofac
{
    using System;
    using Ncqrs.Domain;
    using Ncqrs.Domain.Storage;

    using global::Autofac;

    /// <summary>
    /// Use Autofac to create AggregateRoots for the domain repository. (This allows the domain
    /// repository to re-construct AggregateRoots to their current state, but also have
    /// dependencies injected.
    /// </summary>
    public class AutofacAggregateRootCreationStrategy : AggregateRootCreationStrategy
    {
        private readonly ILifetimeScope _containerScope;

        public AutofacAggregateRootCreationStrategy(ILifetimeScope containerScope)
        {
            _containerScope = containerScope;
        }

        protected override AggregateRoot CreateAggregateRootFromType(Type aggregateRootType)
        {
            return (AggregateRoot)_containerScope.Resolve(aggregateRootType);
        }

    }
}
