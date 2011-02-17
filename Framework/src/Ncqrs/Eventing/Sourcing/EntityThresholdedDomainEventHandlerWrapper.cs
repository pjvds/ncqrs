using System;

namespace Ncqrs.Eventing.Sourcing
{
    public class EntityThresholdedDomainEventHandlerWrapper : ISourcedEventHandler
    {
        private readonly Type _entityType;
        private readonly Guid _entityId;
        private readonly ISourcedEventHandler _wrappedHandler;

        public EntityThresholdedDomainEventHandlerWrapper(Guid entityId, Type entityType, ISourcedEventHandler wrappedHandler)

        {
            _entityId = entityId;
            _entityType = entityType;
            _wrappedHandler = wrappedHandler;
        }

        public bool HandleEvent(object sourcedEvent)
        {
            var sourcedEntityEvent = sourcedEvent as IEntitySourcedEvent;
            if (sourcedEntityEvent == null)
            {
                return false;
            }
            if (sourcedEntityEvent.EntityId != _entityId)
            {
                return false;
            }
            return _wrappedHandler.HandleEvent(sourcedEvent);
        }

        public override string ToString()
        {
            return _entityType.Name + "." + _wrappedHandler.ToString();
        }
    }
}