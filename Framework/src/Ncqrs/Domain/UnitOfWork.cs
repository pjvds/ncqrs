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
    public sealed class UnitOfWork : IUnitOfWorkContext
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// The <see cref="UnitOfWork"/> that is associated with the current thread.
        /// </summary>
        [ThreadStatic]
        private static UnitOfWork _threadInstance;

        /// <summary>
        /// A queue that holds a reference to all instances that have themself registered as a dirty instance during the lifespan of this unit of work instance.
        /// </summary>
        private readonly Queue<AggregateRoot> _dirtyInstances;

        private readonly UncommittedEventStream _eventStream;

        private readonly Action<AggregateRoot, UncommittedEvent> _eventAppliedCallback;

        /// <summary>
        /// A reference to the repository that is associated with this instance.
        /// </summary>
        private readonly IDomainRepository _repository;

        /// <summary>
        /// Gets the <see cref="UnitOfWork"/> associated with the current thread context.
        /// </summary>
        /// <value>The current.</value>
        public static UnitOfWork Current
        {
            get
            {
                return _threadInstance;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the domain repository.
        /// </summary>
        /// <value>The domain repository.</value>
        public IDomainRepository Repository
        {
            get
            {
                return _repository;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="commandId">Id of command being processed.</param>
        /// <param name="domainRepository">The domain repository to use in this unit of work.</param>
        public UnitOfWork(Guid commandId, IDomainRepository domainRepository)
        {
            Contract.Requires<InvalidOperationException>(Current == null, "An other UnitOfWork instance already exists in this context.");
            Contract.Requires<ArgumentNullException>(domainRepository != null);

            Contract.Ensures(_repository == domainRepository, "The _repository member should be initialized with the one given by the domainRepository parameter.");
            Contract.Ensures(_threadInstance == this, "The _threadInstance member should be initialized with this instance.");
            Contract.Ensures(IsDisposed == false);

            Log.DebugFormat("Creating new unit of work for command {0} on thread {1}", commandId,
                            Thread.CurrentThread.ManagedThreadId);

            _repository = domainRepository;
            _eventStream = new UncommittedEventStream(commandId);
            _dirtyInstances = new Queue<AggregateRoot>();
            _threadInstance = this;
            _eventAppliedCallback = new Action<AggregateRoot, UncommittedEvent>(AggregateRootEventAppliedHandler);
            IsDisposed = false;

            InitializeAppliedEventHandler();
        }

        private void InitializeAppliedEventHandler()
        {
            Log.DebugFormat("Registering event applied callback into AggregateRoot from unit of work {0}", this);
            AggregateRoot.RegisterThreadStaticEventAppliedCallback(_eventAppliedCallback);
        }

        private void DestroyAppliedEventHandler()
        {
            Log.DebugFormat("Deregistering event applied callback from AggregateRoot from unit of work {0}", this);
            AggregateRoot.UnregisterThreadStaticEventAppliedCallback(_eventAppliedCallback);
        }

        private void AggregateRootEventAppliedHandler(AggregateRoot aggregateRoot, UncommittedEvent evnt)
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
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="UnitOfWork"/> is reclaimed by garbage collection.
        /// </summary>
        ~UnitOfWork()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Contract.Ensures(IsDisposed == true);

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            Contract.Ensures(IsDisposed == true);

            if (!IsDisposed)
            {
                if (disposing)
                {
                    Log.DebugFormat("Disposing unit of work {0}", this);
                    DestroyAppliedEventHandler();
                    _threadInstance = null;
                }

                IsDisposed = true;
            }
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
        public AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId, long? lastKnownRevision)
        {
            return _repository.GetById(aggregateRootType, eventSourceId, lastKnownRevision);
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

        /// <summary>
        /// Accepts the unit of work and persist the changes.
        /// </summary>
        public void Accept()
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

        public override string ToString()
        {
            return string.Format("{0}@{1}", _eventStream.CommitId, Thread.CurrentThread.ManagedThreadId);
        }
    }
}