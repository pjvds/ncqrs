using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Bus;
using Ncqrs.Denormalization;

namespace Ncqrs.Domain.Denormalization
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
