using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage
{
    public interface IReadableEventStore
    {
        IEnumerable<SourcedEvent> GetEventsSince(Guid eventId, int maxCount);
    }
}