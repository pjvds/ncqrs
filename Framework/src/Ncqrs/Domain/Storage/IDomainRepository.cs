using System;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing;

namespace Ncqrs.Domain.Storage
{
    /// <summary>
    /// A repository that can be used to get and save aggregate roots.
    /// </summary>
    [ContractClass(typeof(IDomainRepositoryContracts))]
    public interface IDomainRepository
    {
        /// <summary>
        /// Gets aggregate root by id.
        /// </summary>
        /// <param name="aggregateRootType">Type of the aggregate root.</param>
        /// <param name="id">The id of the aggregate root.</param>
        /// <returns>A new instance of the aggregate root that contains the latest known state.</returns>
        IAggregateRoot GetById(Type aggregateRootType, Guid id);

        /// <summary>
        /// Gets aggregate root by id.
        /// </summary>
        /// <typeparam name="T">The type of the aggregate root.</typeparam>
        /// <param name="id">The id of the aggregate root.</param>
        /// <returns>A new instance of the aggregate root that contains the latest known state.</returns>
        T GetById<T>(Guid id) where T : IAggregateRoot;

        /// <summary>
        /// Saves the specified aggregate root.
        /// </summary>
        /// <param name="aggregateRootToSave">The aggregate root to save.</param>
        void Save(IEventSource aggregateRootToSave);
    }

    [ContractClassFor(typeof(IDomainRepository))]
    internal sealed class IDomainRepositoryContracts : IDomainRepository
    {
        public IAggregateRoot GetById(Type aggregateRootType, Guid id)
        {
            Contract.Requires<ArgumentNullException>(aggregateRootType != null);

            return null;
        }

        public T GetById<T>(Guid id) where T : IAggregateRoot
        {
            return default(T);
        }

        public void Save(IEventSource aggregateRootToSave)
        {
            Contract.Requires<ArgumentNullException>(aggregateRootToSave != null);
        }
    }

}