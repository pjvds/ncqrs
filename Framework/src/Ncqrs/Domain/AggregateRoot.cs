using System;
using System.Collections.Generic;
using System.Data;
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
        [NonSerialized]
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

                _id = value;
            }
        }

        /// <summary>
        /// Holds entities belonging to this aggregate.
        /// </summary>
        [NonSerialized]
        private readonly Dictionary<Guid, Entity> _entities = new Dictionary<Guid, Entity>();

        /// <summary>
        /// Holds the events that are not yet committed.
        /// </summary>
        [NonSerialized]
        private readonly Queue<DomainEvent> _uncommittedEvent = new Queue<DomainEvent>(0);

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
        [NonSerialized]
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
                Contract.Requires<InvalidOperationException>(Version == InitialVersion);

                _initialVersion = value;
            }
        }

        /// <summary>
        /// A list that contains all the event handlers.
        /// </summary>
        [NonSerialized]
        private readonly List<IDomainEventHandler> _eventHandlers = new List<IDomainEventHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        protected AggregateRoot()
        {
            InitialVersion = 0;

            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            Id = idGenerator.GenerateNewId();
            RegisterEntityCreatedEventHandler();
        }

        private void RegisterEntityCreatedEventHandler()
        {
            _eventHandlers.Add(new TypeThresholdedActionBasedDomainEventHandler(x => OnEntityCreated((EntityCreatedEvent)x), typeof(EntityCreatedEvent) ));
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
        public virtual void InitializeFromHistory(IEnumerable<DomainEvent> history)
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
            if (evnt.EntityId.HasValue)
            {
                DispatchEventToEntity(evnt);
            }
            else
            {
                ProcessEventInRoot(evnt);
            }
        }

        private void ProcessEventInRoot(DomainEvent evnt)
        {
            var handled = _eventHandlers.Aggregate(false, (c, h) => c | h.HandleEvent(evnt));
            if (!handled)
            {
                throw new EventNotHandledException(evnt);
            }
        }

        private void DispatchEventToEntity(DomainEvent evnt)
        {
            Entity target;
            if (!_entities.TryGetValue(evnt.EntityId.Value, out target))
            {
                throw new InvalidOperationException();
            }
            target.HandleEvent(evnt);
        }

        protected void ApplyEvent(DomainEvent evnt)
        {
            ApplyEvent(evnt, false);
        }

        /// <summary>
        /// Called by entities inside this aggregate to inform the root about
        /// events they apply.
        /// </summary>
        /// <param name="evnt">An event applied to an entity.</param>
        public void OnEntityEvent(DomainEvent evnt)
        {
            ValidateEventOwnership(evnt);
            SetEventOwnership(evnt);
            _uncommittedEvent.Enqueue(evnt);
            RegisterCurrentInstanceAsDirty();
        }

        public T GetEntity<T>(Guid id)
            where T : Entity
        {
            return (T)_entities[id];
        }

        /// <summary>
        /// Creates new entity inside this aggregate.
        /// </summary>
        /// <param name="id">Id of newly created entity.</param>
        /// <param name="constructorArguments">Optional constructor arguments.</param>
        /// <returns>New entity instance.</returns>
        public T CreateEntity<T>(Guid id, params object[] constructorArguments)
            where T : Entity
        {
            return (T) CreateEntity(id, typeof (T), constructorArguments);
        }

        /// <summary>
        /// Creates new entity inside this aggregate.
        /// </summary>
        /// <param name="id">Id of newly created entity.</param>
        /// <param name="entityType">Type of entity to create.</param>
        /// <param name="constructorArguments">Optional constructor arguments.</param>
        /// <returns>New entity instance.</returns>
        public Entity CreateEntity(Guid id, Type entityType, params object[] constructorArguments)
        {
            var entityCreatedEvent = new EntityCreatedEvent()
                                         {
                                             Id = id,
                                             EntityType = entityType,
                                             ConstructorArguments = constructorArguments
                                         };
            ApplyEvent(entityCreatedEvent);
            return _entities[id];
        }

        private void OnEntityCreated(EntityCreatedEvent entityCreatedEvent)
        {
            var result = (Entity)Activator.CreateInstance(entityCreatedEvent.EntityType, entityCreatedEvent.ConstructorArguments);
            result.AggregateRoot = this;
            result.Id = entityCreatedEvent.Id;
            _entities[result.Id] = result;
        }

        private void ApplyEvent(DomainEvent evnt, Boolean historical)
        {
            if(historical)
            {
                ValidateEventSequence(evnt);
            }
            else
            {
                ValidateEventOwnership(evnt);
                SetEventOwnership(evnt);
            }

            HandleEvent(evnt);

            if (!historical)
            {
                _uncommittedEvent.Enqueue(evnt);
                RegisterCurrentInstanceAsDirty();
            }
        }

        private void ValidateEventSequence(DomainEvent evnt)
        {
            if(evnt.EventSequence != InitialVersion+1)
            {
                var message = String.Format("Cannot apply event with sequence {0}. Since the initial version of the " +
                                            "aggregate root is {1}. Only an event with sequence number {2} can be applied.",
                                            evnt.EventSequence, InitialVersion, InitialVersion + 1);
                throw new InvalidOperationException(message);
            }
        }

        private void SetEventOwnership(DomainEvent evnt)
        {
            evnt.AggregateRootId = this.Id;
            evnt.EventSequence = Version + 1;
        }

        private void ValidateEventOwnership(DomainEvent evnt)
        {
            if (evnt.AggregateRootId != Guid.Empty)
            {
                var message = String.Format("The {0} event cannot be applied to aggregate root {1} with id {2} " +
                                            "since it was already owned by event aggregate root with id {3}.",
                                            evnt.GetType().FullName, this.GetType().FullName, Id, evnt.AggregateRootId);
                throw new InvalidOperationException(message);
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