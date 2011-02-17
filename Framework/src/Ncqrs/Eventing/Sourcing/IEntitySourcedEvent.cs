using System;

namespace Ncqrs.Eventing.Sourcing
{
    public interface IEntitySourcedEvent
    {
        Guid EntityId { get;}
    }
}
