using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Domain
{
    /// <summary>
    /// The abstract concept of an entity -- an object living inside an aggregate with own thread of identity.
    /// </summary>
    public abstract class Entity : Entity<AggregateRoot>
    {
        protected Entity(AggregateRoot parent, Guid entityId)
            : base(parent, entityId)
        {
        }
    }

    /// <summary>
    /// The abstract concept of an entity -- an object living inside an aggregate with own thread of identity.
    /// </summary>
    public abstract class Entity<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        private readonly Guid _entityId;

        [NonSerialized]
        private readonly TAggregateRoot _parent;

        protected TAggregateRoot ParentAggregateRoot
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets the unique identifier for this entity. This identifier
        /// is set on construction and should not be changed. The creator
        /// of this entity is responsible for providing the right identfier.
        /// </summary>
        public Guid EntityId
        {
            get { return _entityId; }
        }

        protected Entity(TAggregateRoot parent, Guid entityId)
        {
            _parent = parent;
            _entityId = entityId;
        }

        protected void RegisterHandler(ISourcedEventHandler handler)
        {
            _parent.RegisterHandler(handler);
        }

        protected void ApplyEvent(EntitySourcedEventBase evnt)
        {
            // Make sure this event is not already
            // owned by another entity.
            ValidateEventOwnership(evnt);

            evnt.EntityId = EntityId;
            evnt.AggregateId = _parent.EventSourceId;

            _parent.ApplyEvent(evnt);
        }

        private void ValidateEventOwnership(EntitySourcedEventBase evnt)
        {
            if (evnt.EntityId != EntitySourcedEventBase.UndefinedEntityId)
            {
                var message = String.Format("The {0} event cannot be applied to entity {1} with id {2} " +
                            "since it was already owned by entity with id {3}.",
                            evnt.GetType().FullName, this.GetType().FullName, EntityId, evnt.EntityId);
                throw new InvalidOperationException(message);
            }
        }   
    }
}