using System;

namespace Ncqrs.Eventing.Sourcing
{
    public class EntityThresholdedDomainEventHandlerWrapper : ISourcedEventHandler
    {
        private readonly Guid _entityId;
        private readonly ISourcedEventHandler _wrappedHandler;

        public EntityThresholdedDomainEventHandlerWrapper(Guid entityId, ISourcedEventHandler wrappedHandler)

        {
            _entityId = entityId;
            _wrappedHandler = wrappedHandler;
        }

        public bool HandleEvent(ISourcedEvent sourcedEvent)
        {
            var sourcedEntityEvent = sourcedEvent as SourcedEntityEvent;
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
    }
}