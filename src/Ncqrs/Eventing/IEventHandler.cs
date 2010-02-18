using System;

namespace Ncqrs.Eventing
{
    public interface IEventHandler
    {
        void Invoke(IEvent evnt);
    }
}
