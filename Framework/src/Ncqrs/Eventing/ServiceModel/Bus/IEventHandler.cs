using System;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public interface IEventHandler<TEventData> where TEventData : IEvent
    {
        void Handle(IEvent evnt);
    }
}
