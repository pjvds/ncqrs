using System;
using Ncqrs.Eventing;

namespace Ncqrs.Domain
{
    public interface IEventDataHandler<TEventData> where TEventData : IEventData
    {
        bool HandleEventData(TEventData eventData);
    }
}
