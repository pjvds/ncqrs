using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ncqrs.Domain;
using Ncqrs.Eventing;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Spec
{
    public class EventContext : IDisposable
    {
        [ThreadStatic]
        private static EventContext _threadInstance;

        private readonly List<UncommittedEvent> _events = new List<UncommittedEvent>();
        private Action<AggregateRoot, UncommittedEvent> _eventAppliedCallback;

        public IEnumerable<UncommittedEvent> Events
        {
            get { return _events; }
        }

        /// <summary>
        /// Gets the <see cref="EventContext"/> associated with the current thread context.
        /// </summary>
        /// <value>The current.</value>
        public static EventContext Current
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

        public EventContext()
        {
            _threadInstance = this;
            IsDisposed = false;

            InitializeAppliedEventHandler();
        }

        private void InitializeAppliedEventHandler()
        {
            if(_eventAppliedCallback == null)
                _eventAppliedCallback = new Action<AggregateRoot, UncommittedEvent>(AggregateRootEventAppliedHandler);

            AggregateRoot.RegisterThreadStaticEventAppliedCallback(_eventAppliedCallback);
        }

        private void DestroyAppliedEventHandler()
        {
            if(_eventAppliedCallback != null)
                AggregateRoot.UnregisterThreadStaticEventAppliedCallback(_eventAppliedCallback);
        }

        private void AggregateRootEventAppliedHandler(AggregateRoot aggregateRoot, UncommittedEvent evnt)
        {
            _events.Add(evnt);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="UnitOfWork"/> is reclaimed by garbage collection.
        /// </summary>
        ~EventContext()
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
                    DestroyAppliedEventHandler();
                    _threadInstance = null;
                }

                IsDisposed = true;
            }
        }
    }
}
