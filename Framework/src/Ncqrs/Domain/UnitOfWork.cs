using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Threading;
using Ncqrs.Commanding;
using Ncqrs.Domain.Storage;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Domain
{
    public class UnitOfWork : UnitOfWorkBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// A queue that holds a reference to all instances that have themself registered as a dirty instance during the lifespan of this unit of work instance.
        /// </summary>
        private readonly Queue<AggregateRoot> _dirtyInstances;
        private readonly UncommittedEventStream _eventStream;
        private readonly IDomainRepository _repository;       

        public UnitOfWork(Guid commandId, IDomainRepository domainRepository) : base(commandId)
        {
            Contract.Requires<ArgumentNullException>(domainRepository != null);

            Contract.Ensures(_repository == domainRepository, "The _repository member should be initialized with the one given by the domainRepository parameter.");

            _repository = domainRepository;
            _eventStream = new UncommittedEventStream(commandId);
            _dirtyInstances = new Queue<AggregateRoot>();
        }

        protected override void AggregateRootEventAppliedHandler(AggregateRoot aggregateRoot, UncommittedEvent evnt)
        {
            RegisterDirtyInstance(aggregateRoot);            
            _eventStream.Append(evnt);
        }

        [ContractInvariantMethod]
        private void ContractInvariants()
        {
            Contract.Invariant(Contract.ForAll(_dirtyInstances, (instance => instance != null)), "None of the dirty instances can be null.");
        }

        /// <summary>
        /// Gets aggregate root by its id.
        /// </summary>
        /// <param name="aggregateRootType">Type of the aggregate root.</param>
        /// <param name="eventSourceId">The eventSourceId of the aggregate root.</param>
        /// <param name="lastKnownRevision">If specified, the most recent version of event source observed by the client (used for optimistic concurrency).</param>
        /// <returns>
        /// A new instance of the aggregate root that contains the latest known state.
        /// </returns>
        public override AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId, long? lastKnownRevision)
        {
            return _repository.GetById(aggregateRootType, eventSourceId, lastKnownRevision);
        }

        /// <summary>
        /// Accepts the unit of work and persist the changes.
        /// </summary>
        public override void Accept()
        {
            Contract.Requires<ObjectDisposedException>(!IsDisposed);
            Log.DebugFormat("Accepting unit of work {0}", this);
            _repository.Store(_eventStream);
            CreateSnapshots();
        }

        private void CreateSnapshots()
        {
            foreach (AggregateRoot savedInstance in _dirtyInstances)
            {
                _repository.CreateSnapshotIfNecessary(savedInstance);
            }
        }

        /// <summary>
        /// Registers the dirty.
        /// </summary>
        /// <param name="dirtyInstance">The dirty instance.</param>
        private void RegisterDirtyInstance(AggregateRoot dirtyInstance)
        {
            Contract.Requires<ArgumentNullException>(dirtyInstance != null, "dirtyInstance could not be null.");

            if (!_dirtyInstances.Contains(dirtyInstance))
            {
                Log.DebugFormat("Registering aggregate root {0} as dirty in unit of work {1}",
                           dirtyInstance, this);
                _dirtyInstances.Enqueue(dirtyInstance);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}@{1}", _eventStream.CommitId, Thread.CurrentThread.ManagedThreadId);
        }
    }
}