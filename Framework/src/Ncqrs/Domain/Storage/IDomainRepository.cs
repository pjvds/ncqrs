using System;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;

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
        AggregateRoot GetById(Type aggregateRootType, Guid id);

        /// <summary>
        /// Gets aggregate root by id.
        /// </summary>
        /// <typeparam name="T">The type of the aggregate root.</typeparam>
        /// <param name="id">The id of the aggregate root.</param>
        /// <returns>A new instance of the aggregate root that contains the latest known state.</returns>
        T GetById<T>(Guid id) where T : AggregateRoot;

        /// <summary>
        /// Saves the specified aggregate root.
        /// </summary>
        /// <param name="aggregateRootToSave">The aggregate root to save.</param>
        void Save(AggregateRoot aggregateRootToSave);
    }

    [ContractClassFor(typeof(IDomainRepository))]
    internal sealed class IDomainRepositoryContracts : IDomainRepository
    {
        public AggregateRoot GetById(Type aggregateRootType, Guid id)
        {
            Contract.Requires<ArgumentNullException>(aggregateRootType != null);

            return default(AggregateRoot);
        }

        public T GetById<T>(Guid id) where T : AggregateRoot
        {
            return default(T);
        }

        public void Save(AggregateRoot aggregateRootToSave)
        {
            Contract.Requires<ArgumentNullException>(aggregateRootToSave != null);
        }
    }

}