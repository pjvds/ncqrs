using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain
{
    /// <summary>
    /// The abstract concept of an entity.
    /// </summary>
    public abstract class Entity
    {
        protected internal AggregateRoot AggregateRoot { get; internal set;}

        [NonSerialized]
        private Guid _id;

        /// <summary>
        /// Gets the aggregate-scoped unique identifier.
        /// </summary>        
        public Guid Id
        {
            get { return _id; }
            internal set
            {
                _id = value;
            }
        }

        /// <summary>
        /// A list that contains all the event handlers.
        /// </summary>
        [NonSerialized]
        private readonly List<IDomainEventHandler> _eventHandlers = new List<IDomainEventHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        protected Entity()
        {
            var idGenerator = NcqrsEnvironment.Get<IUniqueIdentifierGenerator>();
            Id = idGenerator.GenerateNewId();
        }

        protected void RegisterHandler(IDomainEventHandler handler)
        {
            Contract.Requires<ArgumentNullException>(handler != null, "The handler cannot be null.");
            _eventHandlers.Add(handler);
        }

        public virtual void HandleEvent(DomainEvent evnt)
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
            if (!historical)            
            {
                ValidateEventOwnership(evnt);
                SetEventOwnership(evnt);
                AggregateRoot.OnEntityEvent(evnt);
            }

            HandleEvent(evnt);
        }

        private void SetEventOwnership(DomainEvent evnt)
        {
            evnt.EntityId = Id;
        }

        private void ValidateEventOwnership(DomainEvent evnt)
        {
            if (evnt.EntityId.HasValue)
            {
                var message = String.Format("The {0} event cannot be applied to entity {1} with id {2} " +
                                            "since it was already owned by entity with id {3}.",
                                            evnt.GetType().FullName, GetType().FullName, Id, evnt.EntityId);
                throw new InvalidOperationException(message);
            }
        }
    }
}