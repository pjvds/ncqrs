using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ncqrs.Eventing.Sourcing
{
    public interface ISourcedEntityEvent : ISourcedEvent
    {
        Guid EntityId { get;}
    }

    internal interface  IAllowSettingEntityId
    {
        void SetEntityId(Guid entityId);
    }
}
