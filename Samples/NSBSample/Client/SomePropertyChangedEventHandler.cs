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
         ClientEndpoint.AggregateId = message.Payload.AggregateRootId;
         Console.WriteLine("Aggregate with ID={0} property changed to {1}", message.Payload.AggregateRootId, message.Payload.Value);         
      }
   }
}