using System;
using Ncqrs.Domain;

namespace Events
{
   [Serializable]
   public class SomeDomainObjectCreatedEvent : DomainEvent
   {
      public Guid ObjectId { get; set; }
   }
}