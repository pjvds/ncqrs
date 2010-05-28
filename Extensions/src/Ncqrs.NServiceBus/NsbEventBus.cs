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
         Bus.Publish(CreateEventMessage(eventMessage));
      }

      public void Publish(IEnumerable<IEvent> eventMessages)
      {
         Bus.Publish(eventMessages.Select(CreateEventMessage).ToArray());
      }

      public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
      {
      }

      private static IBus Bus
      {
         get { return NcqrsEnvironment.Get<IBus>(); }
      }

      private static IMessage CreateEventMessage(IEvent payload)
      {
         Type factoryType =
            typeof (EventMessageFactory<>).MakeGenericType(payload.GetType());
         var factory =
            (IEventMessageFactory) Activator.CreateInstance(factoryType);
         return factory.CreateEventMessage(payload);
      }

      public interface IEventMessageFactory
      {
         IMessage CreateEventMessage(IEvent payload);
      }

      private class EventMessageFactory<T> : IEventMessageFactory where T : IEvent
      {
         IMessage IEventMessageFactory.CreateEventMessage(IEvent payload)
         {
            return new EventMessage<T>
                      {
                         Payload = (T)payload
                      };
         }
      }
   }
}
