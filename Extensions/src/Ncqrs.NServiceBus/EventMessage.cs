using System;
using Ncqrs.Eventing;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
   [Serializable]
   public class EventMessage<T> : IMessage
      where T : IEvent
   {
      public T Payload { get; set; }
   }
}