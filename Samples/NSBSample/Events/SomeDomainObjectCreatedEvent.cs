using System;
using Ncqrs.Eventing.Sourcing;

namespace Events
{
   [Serializable]
   public class SomeDomainObjectCreatedEvent : SourcedEvent
   {
      public Guid ObjectId { get; set; }
   }
}