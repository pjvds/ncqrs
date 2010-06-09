using System;

namespace Ncqrs.Domain
{
    /// <summary>
    /// A built-in event raised when new entity is created inside the aggregate.
    /// </summary>
    [Serializable]
    public class EntityCreatedEvent : DomainEvent
    {
        /// <summary>
        /// Gets or sets new entity id.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets new entity type.
        /// </summary>
        public Type EntityType { get; set; }
        /// <summary>
        /// Gets or sets constructor arguments used to create entity instance.
        /// </summary>
        public object[] ConstructorArguments { get; set; }

        public EntityCreatedEvent()
        {
        }

        public EntityCreatedEvent(Guid id, Type entityType, object[] constructorArguments, 
            Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp)
            : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
        {
            Id = id;
            EntityType = entityType;
            ConstructorArguments = constructorArguments;
        }
    }
}