using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    [EventName("MyCompany:AwesomeApp:Events:PersonCreated")]
    public class PersonCreatedEvent
    {
        public string Forename { get; set; }
        public string Surname { get; set; }

        public PersonCreatedEvent()
        {
        }

        public PersonCreatedEvent(string forename, string surname) : this()
        {
            Forename = forename;
            Surname = surname;
        }
    }
}
