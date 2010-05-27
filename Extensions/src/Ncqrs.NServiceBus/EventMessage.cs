using System;
using Ncqrs.Eventing;
using NServiceBus;

namespace Ncqrs.NServiceBus
{
   [Serializable]
   public class EventMessage : IMessage
   {
      public IEvent Payload { get; set; }
   }
}