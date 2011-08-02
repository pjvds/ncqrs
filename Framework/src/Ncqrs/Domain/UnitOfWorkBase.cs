using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Threading;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    public abstract class UnitOfWorkBase : IUnitOfWorkContext
    {
        private readonly Guid _commandId;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Action<AggregateRoot, UncommittedEvent> _eventAppliedCallback;

        /// <summary>
        /// Gets the id of command which triggered this unit of work.
        /// </summary>
        protected Guid CommandId
        {
            get { return _commandId; }
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
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="commandId">Id of command being processed.</param>
        protected UnitOfWorkBase(Guid commandId)
        {
            _commandId = commandId;
            Contract.Ensures(IsDisposed == false);

            Log.DebugFormat("Creating new unit of work for command {0} on thread {1}", commandId,
                            Thread.CurrentThread.ManagedThreadId);

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

        protected abstract void AggregateRootEventAppliedHandler(AggregateRoot aggregateRoot, UncommittedEvent evnt);

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="UnitOfWork"/> is reclaimed by garbage collection.
        /// </summary>
        ~UnitOfWorkBase()
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
        protected virtual void Dispose(bool disposing)
        {
            Contract.Ensures(IsDisposed == true);

            if (!IsDisposed)
            {
                if (disposing)
                {
                    Log.DebugFormat("Disposing unit of work {0}", this);
                    DestroyAppliedEventHandler();
                    UnitOfWorkContext.Unbind();
                }

                IsDisposed = true;
            }
        }

               

        

        public abstract AggregateRoot GetById(Type aggregateRootType, Guid eventSourceId, long? lastKnownRevision);
        public TAggregateRoot GetById<TAggregateRoot>(Guid eventSourceId, long? lastKnownRevision) where TAggregateRoot : AggregateRoot
        {
            return (TAggregateRoot) GetById(typeof (TAggregateRoot), eventSourceId, lastKnownRevision);
        }

        public TAggregateRoot GetById<TAggregateRoot>(Guid eventSourceId) where TAggregateRoot : AggregateRoot
        {
            return GetById<TAggregateRoot>(eventSourceId, null);
        }

        public abstract void Accept();
    }
}