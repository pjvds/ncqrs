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
        private Guid _id;

        /// <summary>
        /// Gets the globally unique identifier.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when setting this
        /// value when the version of this aggregate root is not 0 or this
        /// instance contains are any uncommitted events.</exception>
        public Guid Id
        {
            get { return _id; }
            protected set
            {
                Contract.Requires<InvalidOperationException>(Version == 0);
                Contract.Requires<InvalidOperationException>(_uncommittedEvent.Count == 0);

                _id = value;
            }
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
            Version = 0;

            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            Id = idGenerator.GenerateNewId();
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

            foreach (var handler in _eventHandlers)
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
            HandleEvent(evnt);

            if (!historical)
            {
                if (evnt.AggregateRootId != Guid.Empty)
                {
                    var message = "The {0} event cannot be applied to aggregate root {1} with id {2} "
                                  + "since it was already owned by event aggregate root with id {3}."
                                  .FormatWith(evnt.GetType().FullName, this.GetType().FullName, Id, evnt.AggregateRootId);
                    throw new InvalidOperationException(message);
                }

                // TODO: Validate id.
                evnt.AggregateRootId = this.Id;
                _uncommittedEvent.Push(evnt);
            }

            OnEventApplied(evnt);
        }

        public IEnumerable<DomainEvent> GetUncommitedEvents()
        {
            Contract.Ensures(Contract.Result<IEnumerable<DomainEvent>>() != null, "The result of this method should never be null.");

            return _uncommittedEvent.ToArray();
        }

        IEnumerable<ISourcedEvent> IEventSource.GetUncommittedEvents()
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