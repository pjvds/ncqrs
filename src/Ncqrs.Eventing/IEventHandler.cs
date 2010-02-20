using System;

namespace Ncqrs.Eventing
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        void Handle(TEvent evnt);
    }
}