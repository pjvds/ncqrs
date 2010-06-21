using System;
using Ncqrs.Eventing.Sourcing;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
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
