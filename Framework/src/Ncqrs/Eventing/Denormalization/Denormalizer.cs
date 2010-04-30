using System;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Eventing.Denormalization
{
    public abstract class Denormalizer<TEvent> : IDenormalizer, IEventHandler where TEvent : IEvent
    {
        public abstract void DenormalizeEvent(TEvent evnt);

        void IEventHandler.Handle(IEvent evnt)
        {
            ((IDenormalizer)this).DenormalizeEvent(evnt);
        }

        void IDenormalizer.DenormalizeEvent(IEvent evnt)
        {
            if (evnt is TEvent)
            {
                DenormalizeEvent((TEvent)evnt);
            }
        }
    }
}
