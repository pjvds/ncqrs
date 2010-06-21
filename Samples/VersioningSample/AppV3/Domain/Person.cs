using System;
using AwesomeAppRefactored.Events;
using Ncqrs.Domain;

namespace AwesomeAppRefactored.Domain
{
    public class Person : AggregateRootMappedByConvention
    {
        private string name;

        protected Person()
        {
        }

        public Person(Guid id, string name)
        {
            EventSourceId = id;
            var e = new PersonCreatedEvent(name);
            ApplyEvent(e);
        }

        public void ChangeName(string name)
        {
            var e = new NameChangedEvent(name);
            ApplyEvent(e);
        }

        protected void OnPersonCreated(PersonCreatedEvent e)
        {
            name = e.Name;
        }

        protected void OnNameChanged(NameChangedEvent e)
        {
            name = e.Name;
        }
    }
}
