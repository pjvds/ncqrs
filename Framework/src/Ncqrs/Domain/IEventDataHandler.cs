using System;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    public interface IEventDataHandler<TEventData> where TEventData : IEvent
    {
        bool HandleEventData(TEventData evnt);
    }
}
