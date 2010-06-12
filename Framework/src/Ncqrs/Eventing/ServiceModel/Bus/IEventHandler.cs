using System;

namespace Ncqrs.Eventing.ServiceModel.Bus
{
    public interface IEventHandler<TEventData> where TEventData : IEventData
    {
        void Handle(IEvent<TEventData> evnt);
    }
}
