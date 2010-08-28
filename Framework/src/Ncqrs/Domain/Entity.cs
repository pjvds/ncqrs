using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Domain
{
    /// <summary>
    /// The abstract concept of an entity -- an object living inside an aggregate with own thread of identity.
    /// </summary>
    public abstract class Entity
    {
        private readonly Guid _entityId;

        [NonSerialized]
        private readonly AggregateRoot _parent;

        /// <summary>
        /// Gets the unique identifier for this entity. This identifier
        /// is set on construction and should not be changed. The creator
        /// of this entity is responsible for providing the right identfier.
        /// </summary>
        public Guid EntityId
        {
            get { return _entityId; }
        }

        protected Entity(AggregateRoot parent, Guid entityId)
        {
            _parent = parent;
            _entityId = entityId;
        }

        protected void RegisterHandler(ISourcedEventHandler handler)
        {
            _parent.RegisterHandler(handler);
        }

        protected void ApplyEvent(SourcedEntityEvent evnt)
        {
            // Make sure this event is not already
            // owned by another entity.
            ValidateEventOwnership(evnt);

            evnt.EntityId = EntityId;

            _parent.ApplyEvent(evnt);
        }

        private void ValidateEventOwnership(SourcedEntityEvent evnt)
        {
            if (evnt.EntityId != SourcedEntityEvent.UndefinedEventSourceId)
            {
                var message = String.Format("The {0} event cannot be applied to entity {1} with id {2} " +
                            "since it was already owned by entity with id {3}.",
                            evnt.GetType().FullName, this.GetType().FullName, EntityId, evnt.EntityId);
                throw new InvalidOperationException(message);
            }
        }
    }
}