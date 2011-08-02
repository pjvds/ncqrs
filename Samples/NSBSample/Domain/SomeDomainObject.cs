using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Events;
using Ncqrs.Domain;

namespace Domain
{
    public class SomeDomainObject : AggregateRootMappedByConvention
    {
        private string _value;

        public string Value
        {
            get { return _value; }
        }

        public SomeDomainObject(int dummyValue)
        {
            Console.WriteLine("SomeDomainObject with ID={0} created!", EventSourceId);
            ApplyEvent(new SomeDomainObjectCreatedEvent() { ObjectId = EventSourceId });
        }

        public void DoSomething(string value)
        {
            Console.WriteLine("Calling DoSomething on SomeDomainObject with ID={0}", EventSourceId);
            ApplyEvent(new SomePropertyChangedEvent { Value = value });
        }

        private void OnSomePropertyChangedEvent(SomePropertyChangedEvent @event)
        {
            _value = @event.Value;
        }

        private void OnSomeDomainObjectCreatedEvent(SomeDomainObjectCreatedEvent @event)
        {            
        }

        public SomeDomainObject()
        {
        }
    }
}
