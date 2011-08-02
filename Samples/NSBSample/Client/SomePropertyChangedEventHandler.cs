using System;
using Events;
using Ncqrs.NServiceBus;
using NServiceBus;

namespace Client
{
   public class SomePropertyChangedEventHandler : IMessageHandler<EventMessage<SomePropertyChangedEvent>>
   {
      public void Handle(EventMessage<SomePropertyChangedEvent> message)
      {
         Console.WriteLine("Property changed to {0}", message.Payload.Value);         
      }
   }
}