using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Mapping;

namespace Ncqrs.Domain
{
    public abstract class EntityMappedByConvention : Entity
    {
        protected EntityMappedByConvention(AggregateRoot parent, Guid entityId) : base(parent, entityId)
        {
            var mapping = new ConventionBasedEventHandlerMappingStrategy();
            var handlers = mapping.GetEventHandlers(this);

            foreach(var directHandler in handlers)
            {
                var handler = new SourcedEventHandlerPredicate<SourcedEntityEvent>((e) => e.EntityId == this.EntityId,
                                                                                   directHandler);

                parent.RegisterHandler(handler);
            }
        }
    }
}
