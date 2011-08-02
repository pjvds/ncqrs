using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Mapping;

namespace Ncqrs.Domain
{
    public abstract class EntityMappedByConvention : EntityMappedByConvention<AggregateRoot>
    {
        protected EntityMappedByConvention(AggregateRoot parent, Guid entityId) : base(parent, entityId)
        {
        }
    }

    public abstract class EntityMappedByConvention<TAggregateRoot> : Entity<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        protected EntityMappedByConvention(TAggregateRoot parent, Guid entityId)
            : base(parent, entityId)
        {
            var mapping = new ConventionBasedEventHandlerMappingStrategy();
            var handlers = mapping.GetEventHandlers(this);

            foreach(var directHandler in handlers)
            {
                var handler = new EntityThresholdedDomainEventHandlerWrapper(EntityId, GetType(), directHandler);

                parent.RegisterHandler(handler);
            }
        }
    }
}
