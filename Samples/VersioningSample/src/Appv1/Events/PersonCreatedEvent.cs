using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace AwesomeApp.Events
{
    [Serializable]
    [EventName("MyCompany:AwesomeApp:Events:PersonCreated@1")]
    public class PersonCreatedEvent : SourcedEvent
    {
        public string Forename { get; private set; }
        public string Surname { get; private set; }

        public PersonCreatedEvent()
        {
        }

        public PersonCreatedEvent(string forename, string surname)
        {
            Forename = forename;
            Surname = surname;
        }
    }
}
