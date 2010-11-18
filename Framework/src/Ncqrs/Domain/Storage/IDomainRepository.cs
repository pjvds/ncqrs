using System;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Storage
{
    /// <summary>
    /// A repository that can be used to get and save aggregate roots.
    /// </summary>
    [ContractClass(typeof(IDomainRepositoryContracts))]
    public interface IDomainRepository
    {
        /// <summary>
        /// Gets aggregate root by eventSourceId.
        /// </summary>
        /// <param name="aggregateRootType">Type of the aggregate root.</param>
        /// <param name="eventSourceId">The eventSourceId of the aggregate root.</param>
        /// <returns>A new instance of the aggregate root that contains the latest known state.</returns>
        AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId);

        /// <summary>
        /// Gets aggregate root by eventSourceId. If aggregate root with this id does not exist, returns null.
        /// </summary>
        /// <param name="aggregateRootType">Type of the aggregate root.</param>
        /// <param name="eventSourceId">The eventSourceId of the aggregate root.</param>
        /// <returns>A new instance of the aggregate root that contains the latest known state or null</returns>
        AggregateRoot TryGetById(Type aggregateRootType, Guid eventSourceId);

        /// <summary>
        /// Gets aggregate root by eventSourceId.
        /// </summary>
        /// <typeparam name="T">The type of the aggregate root.</typeparam>
        /// <param name="eventSourceId">The eventSourceId of the aggregate root.</param>
        /// <returns>A new instance of the aggregate root that contains the latest known state.</returns>
        T GetById<T>(Guid eventSourceId) where T : AggregateRoot;

        /// <summary>
        /// Saves the specified aggregate root.
        /// </summary>
        /// <param name="aggregateRootToSave">The aggregate root to save.</param>
        void Save(AggregateRoot aggregateRootToSave);
    }

    [ContractClassFor(typeof(IDomainRepository))]
    internal abstract class IDomainRepositoryContracts : IDomainRepository
    {
        public AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId)
        {
            Contract.Requires<ArgumentNullException>(aggregateRootType != null);

            return default(AggregateRoot);
        }

        public AggregateRoot TryGetById(Type aggregateRootType, Guid eventSourceId)
        {
            Contract.Requires<ArgumentNullException>(aggregateRootType != null);

            return default(AggregateRoot);
        }

        public T GetById<T>(Guid eventSourceId) where T : AggregateRoot
        {
            return default(T);
        }

        public void Save(AggregateRoot aggregateRootToSave)
        {
            Contract.Requires<ArgumentNullException>(aggregateRootToSave != null);
        }
    }

}