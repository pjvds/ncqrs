using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Events;
using Ncqrs.Domain;

namespace Domain
{
   public class SomeDomainObject : AggregateRoot
   {
      private string _value;

      public string Value
      {
         get { return _value; }
      }

      public SomeDomainObject()
      {
         Console.WriteLine("SomeDomainObject created!");
      }

      public void DoSomething(string value)
      {
         ApplyEvent(new SomePropertyChangedEvent{Value = value});
      }

      private void OnSomePropertyChanged(SomePropertyChangedEvent @event)
      {
         _value = @event.Value;
      }
   }
}
