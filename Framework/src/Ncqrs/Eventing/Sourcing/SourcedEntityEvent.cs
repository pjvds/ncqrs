using System;

namespace Ncqrs.Eventing.Sourcing
{
    public class SourcedEntityEvent : SourcedEvent
    {
        public static Guid UndefinedEntityId = Guid.Empty;

        /// <summary>
        /// Gets the id of the entity that causes this event.
        /// </summary>
        public Guid EntityId { get; internal set; }


        public SourcedEntityEvent()
        {
        }

        public SourcedEntityEvent(Guid eventIdentifier, Guid eventSourceId, Guid entityId, long eventSequence, DateTime eventTimeStamp) : base(eventIdentifier, eventSourceId, eventSequence, eventTimeStamp)
        {
            EntityId = entityId;
        }
    }
}
