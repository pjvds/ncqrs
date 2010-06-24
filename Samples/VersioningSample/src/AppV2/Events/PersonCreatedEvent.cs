using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    [EventName("MyCompany:AwesomeApp:Events:PersonCreated")]
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
