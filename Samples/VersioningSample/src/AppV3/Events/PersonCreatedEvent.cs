using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    [EventName("MyCompany:AwesomeApp:Events:PersonCreated")]
    public class PersonCreatedEvent : SourcedEvent
    {
        public string Name { get; set; }

        public PersonCreatedEvent()
        {
            EventVersion = new Version(2, 0);
        }

        public PersonCreatedEvent(string name) : this()
        {
            Name = name;
        }
    }
}
