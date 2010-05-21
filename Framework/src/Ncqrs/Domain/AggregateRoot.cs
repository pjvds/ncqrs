using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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
        /// Gets the current version of the instance as it is known in the event store.
        /// </summary>
        /// <value>
        /// An <see cref="long"/> representing the current version of this aggregate root.
        /// </value>
        public long Version
        {
            get
            {
                return InitialVersion + _uncommittedEvent.Count;
            }
        }

        private long _initialVersion;

        /// <summary>
        /// Gets the initial version.
        /// <para>
        /// This represents the current version of this instance. When this instance was retrieved
        /// via history, it contains the version as it was at that time. For new instances this value is always 0.
        /// </para>
        /// 	<para>
        /// The version does not change until changes are accepted via the <see cref="AcceptChanges"/> method.
        /// </para>
        /// </summary>
        /// <value>The initial version.</value>
        public long InitialVersion
        {
            get { return _initialVersion; }
            protected set
            {
                Contract.Requires<InvalidOperationException>(_uncommittedEvent.Count == 0);

                _initialVersion = value;
            }
        }

        /// <summary>
        /// A list that contains all the event handlers.
        /// </summary>
        private readonly List<IDomainEventHandler> _eventHandlers = new List<IDomainEventHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        protected AggregateRoot()
        {
            InitialVersion = 0;

            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            Id = idGenerator.GenerateNewId();
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
        protected internal virtual void InitializeFromHistory(IEnumerable<DomainEvent> history)
        {
            Contract.Requires<ArgumentNullException>(history != null, "The history cannot be null.");
            if (_uncommittedEvent.Count > 0) throw new InvalidOperationException("Cannot apply history when instance has uncommitted changes.");

            foreach (var historicalEvent in history)
            {
                ApplyEvent(historicalEvent, true);
                InitialVersion++; // TODO: Thought... couldn't we get this from the event?
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
        }

        protected void ApplyEvent(DomainEvent evnt)
        {
            ApplyEvent(evnt, false);
        }

        private void ApplyEvent(DomainEvent evnt, Boolean historical)
        {
            if(historical)
            {
                if(evnt.EventSequence != InitialVersion+1)
                {
                    var message = String.Format("Cannot apply event with sequence {0}. Since the initial version of the " +
                                                "aggregate root is {1}. Only an event with sequence number {2} can be applied.",
                                                evnt.EventSequence, InitialVersion, InitialVersion + 1);
                    throw new InvalidOperationException(message);
                }
            }
            else
            {
                if (evnt.AggregateRootId != Guid.Empty)
                {
                    var message = String.Format("The {0} event cannot be applied to aggregate root {1} with id {2} " +
                                                "since it was already owned by event aggregate root with id {3}.",
                                                evnt.GetType().FullName, this.GetType().FullName, Id, evnt.AggregateRootId);
                    throw new InvalidOperationException(message);
                }

                evnt.AggregateRootId = this.Id;
                evnt.EventSequence = Version + _uncommittedEvent.Count + 1;
            }

            HandleEvent(evnt);

            if (!historical)
            {
                _uncommittedEvent.Push(evnt);
                RegisterCurrentInstanceAsDirty();
            }
        }

        public IEnumerable<DomainEvent> GetUncommittedEvents()
        {
            Contract.Ensures(Contract.Result<IEnumerable<DomainEvent>>() != null, "The result of this method should never be null.");

            return _uncommittedEvent.ToArray();
        }

        IEnumerable<ISourcedEvent> IEventSource.GetUncommittedEvents()
        {
            // TODO: .net 4.0 co/con
            return GetUncommittedEvents().Cast<ISourcedEvent>();
        }

        public void AcceptChanges()
        {
            // Get current version, since when we clear 
            // the uncommited event stack the version will 
            // be InitialVersion+0. Since the version is 
            // the result of InitialVersion+number of uncommited events.
            long newInitialVersion = Version;

            _uncommittedEvent.Clear();

            this.InitialVersion = newInitialVersion;
        }

        private void RegisterCurrentInstanceAsDirty()
        {
            if(UnitOfWork.Current == null)
                throw new NoUnitOfWorkAvailableInThisContextException();

            UnitOfWork.Current.RegisterDirtyInstance(this);
        }
    }
}