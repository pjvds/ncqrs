using System;
using Ncqrs.Eventing.Sourcing;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    public class PersonCreatedEvent : SourcedEvent
    {
        public string Forename { get; set; }
        public string Surname { get; set; }

        public PersonCreatedEvent()
        {
            EventVersion = new Version(1, 1);
        }

        public PersonCreatedEvent(string forename, string surname) : this()
        {
            Forename = forename;
            Surname = surname;
        }
    }
}
