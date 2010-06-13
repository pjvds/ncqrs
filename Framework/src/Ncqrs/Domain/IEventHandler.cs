using System;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        bool HandleEventData(TEvent evnt);
    }
}
