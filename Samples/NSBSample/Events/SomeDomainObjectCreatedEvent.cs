using System;

namespace Events
{
   [Serializable]
   public class SomeDomainObjectCreatedEvent
   {
      public Guid ObjectId { get; set; }
   }
}