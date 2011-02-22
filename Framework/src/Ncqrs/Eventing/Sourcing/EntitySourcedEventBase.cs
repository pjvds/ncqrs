using System;

namespace Ncqrs.Eventing.Sourcing
{
    public abstract class EntitySourcedEventBase : IEntitySourcedEvent
    {
        public static Guid UndefinedEntityId = Guid.Empty;

        /// <summary>
        /// Gets or sets the id of the entity that causes this event.
        /// </summary>
        public Guid EntityId { get; set; }
        public Guid AggregateId { get; set; }
    }
}
