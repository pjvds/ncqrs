using System;
using Ncqrs.Eventing.Sourcing;

namespace Events
{
   [Serializable]
   public class SomePropertyChangedEvent : SourcedEvent

   {
      public string Value { get; set; }
   }
}
