using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;

namespace Events
{
   [Serializable]
   public class SomePropertyChangedEvent : DomainEvent
   {
      public string Value { get; set; }
   }
}
