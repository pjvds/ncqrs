using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Bus;

namespace Ncqrs.Eventing.Denormalization
{
    public abstract class Denormalizer<TEvent> : IEventHandler where TEvent : IEvent
    {
        public abstract void DenormalizeEvent(TEvent evnt);

        void IEventHandler.Handle(IEvent evnt)
        {
            DenormalizeEvent((TEvent)evnt);
        }
    }
}
