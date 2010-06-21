using System;
using AwesomeApp.Events;
using Ncqrs.Domain;

namespace AwesomeApp.Domain
{
    public class Person : AggregateRootMappedByConvention
    {
        private string forename;
        private string surname;

        protected Person()
        {
        }

        public Person(Guid id, string forename, string surname)
        {
            EventSourceId = id;
            var e = new PersonCreatedEvent(forename, surname);
            ApplyEvent(e);
        }

        public void ChangeName(string forename, string surname)
        {
            var e = new NameChangedEventttt(forename, surname);
            ApplyEvent(e);
        }

        protected void OnPersonCreated(PersonCreatedEvent e)
        {
            forename = e.Forename;
            surname = e.Surname;
        }

        protected void OnNameChanged(NameChangedEventttt e)
        {
            forename = e.Forename;
            surname = e.Surname;
        }
    }
}
