using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
   public class NsbEventBus : IEventBus
   {
      public void Publish(IEvent eventMessage)
      {
         Bus.Publish(new EventMessage {Payload = eventMessage});
      }

      public void Publish(IEnumerable<IEvent> eventMessages)
      {
         Bus.Publish(eventMessages.Select(x => new EventMessage { Payload = x }).ToArray());
      }

      public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
      {
      }

      private static IBus Bus
      {
         get { return NcqrsEnvironment.Get<IBus>(); }
      }
   }
}
