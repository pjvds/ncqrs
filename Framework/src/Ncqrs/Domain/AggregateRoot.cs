using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Ncqrs.Domain.Mapping;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    /// <summary>
    /// The abstract concept of an aggregate root.
    /// </summary>
    public abstract class AggregateRoot : IEventSource
    {
        /// <summary>
        /// Gets the globally unique identifier.
        /// </summary>
        public Guid Id
        {
            get;
            protected set; // TODO: Only allow ID change when there are no events.
        }

        /// <summary>
        /// Holds the events that are not yet committed.
        /// </summary>
        private readonly Stack<DomainEvent> _uncommittedEvent = new Stack<DomainEvent>(0);

        /// <summary>
        /// Gets the current version.
        /// </summary>
        /// <value>An <see cref="int"/> representing the current version of this aggregate root.</value>
        public long Version
        {
            get;
            private set;
        }

        /// <summary>
        /// A list that contains all the event handlers.
        /// </summary>
        private readonly List<IDomainEventHandler> _eventHandlers = new List<IDomainEventHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <remarks>
        /// This instance will be initialized with the <see cref="BasicGuidGenerator"/>.
        /// </remarks>
        protected AggregateRoot()
        {
            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            Id = idGenerator.GenerateNewId();
            Version = 0;
        }

        protected AggregateRoot(IEnumerable<DomainEvent> history)
        {
            InitializeFromHistory(history);
        }

        [ContractInvariantMethod]
        private void ContractInvariants()
        {
            Contract.Invariant(_uncommittedEvent != null, "The member _unacceptedEvents should never be null.");
        }

        /// <summary>
        /// Initializes from history.
        /// </summary>
        /// <param name="history">The history.</param>
        protected virtual void InitializeFromHistory(IEnumerable<DomainEvent> history)
        {
            if (history == null) throw new ArgumentNullException("history");
            if (history.Count() == 0)
                throw new ArgumentException("The provided history does not contain any historical event.", "history");
            if (Version != 0 || _uncommittedEvent.Count > 0)
                throw new InvalidOperationException("Cannot load from history when a event source is already loaded.");

            foreach (var historicalEvent in history)
            {
                ApplyEvent(historicalEvent, true);
                Version++;
            }
        }

        protected void RegisterHandler(IDomainEventHandler handler)
        {
            Contract.Requires<ArgumentNullException>(handler != null, "The handler cannot be null.");

            _eventHandlers.Add(handler);
        }

        protected virtual void HandleEvent(DomainEvent evnt)
        {
            Contract.Requires<ArgumentNullException>(evnt != null, "The evnt cannot be null.");
            Boolean handled = false;

            foreach(var handler in _eventHandlers)
            {
                handled |= handler.HandleEvent(evnt);
            }

            if (!handled)
                throw new EventNotHandledException(evnt);

            // TODO: Add validation to make sure this ID isn't already set.
            evnt.AggregateRootId = this.Id;
        }

        protected void ApplyEvent(DomainEvent evnt)
        {
            ApplyEvent(evnt, false);
        }

        private void ApplyEvent(DomainEvent evnt, Boolean historical)
        {
            if (evnt == null) throw new ArgumentNullException("evnt");
            HandleEvent(evnt);

            if(!historical)
                _uncommittedEvent.Push(evnt);

            OnEventApplied(evnt);
        }

        public IEnumerable<DomainEvent> GetUncommitedEvents()
        {
            Contract.Ensures(Contract.Result<IEnumerable<DomainEvent>>() != null, "The result of this method should never be null.");

            return _uncommittedEvent;
        }

        IEnumerable<IEventSourcedEvent> IEventSource.GetUncommittedEvents()
         {
             return GetUncommitedEvents();
         }

        public void CommitEvents()
        {
            // Clear the unaccepted event list.
            _uncommittedEvent.Clear();
        }

        [NoEventHandler]
        protected void OnEventApplied(DomainEvent evnt)
        {
            // Register this instance as a dirty one.
            var unitOfWorkFactory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            var currentUnitOfWork = unitOfWorkFactory.GetUnitOfWorkInCurrentContext();
            
            currentUnitOfWork.RegisterDirtyInstance(this);
        }
    }
}