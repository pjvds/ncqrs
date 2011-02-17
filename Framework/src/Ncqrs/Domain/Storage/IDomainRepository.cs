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
        /// Gets aggregate root by eventSourceId.
        /// </summary>
        /// <param name="aggregateRootType">Type of the aggregate root.</param>
        /// <param name="eventSourceId">The eventSourceId of the aggregate root.</param>
        /// <param name="lastKnownRevision">If specified, the most recent version of event source observed by the client (used for optimistic concurrency).</param>
        /// <returns>A new instance of the aggregate root that contains the latest known state or null if aggregate does not exist.</returns>
        AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId, long? lastKnownRevision);

        /// <summary>
        /// Saves the specified aggregate root.
        /// </summary>
        /// <param name="eventStream">The stream of events to persist.</param>
        void Store(UncommittedEventStream eventStream);

        /// <summary>
        /// Creates snapshot if necessary.
        /// </summary>
        /// <param name="aggregateRoot">Aggregate root to be snapshotted.</param>
        void CreateSnapshotIfNecessary(AggregateRoot aggregateRoot);
    }

    [ContractClassFor(typeof(IDomainRepository))]
    internal abstract class IDomainRepositoryContracts : IDomainRepository
    {
        public AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId, long ? lastKnownRevision)
        {
            Contract.Requires<ArgumentNullException>(aggregateRootType != null);

            return default(AggregateRoot);
        }

        public void Store(UncommittedEventStream eventStream)
        {
            Contract.Requires<ArgumentNullException>(eventStream != null);
        }

        public void CreateSnapshotIfNecessary(AggregateRoot aggregateRoot)
        {
            Contract.Requires<ArgumentNullException>(aggregateRoot != null);
        }
    }

}