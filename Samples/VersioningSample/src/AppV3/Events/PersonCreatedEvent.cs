using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    [EventName("MyCompany:AwesomeApp:Events:PersonCreated@2")]
    [EventNameAlias("MyCompany:AwesomeApp:Events:PersonCreated@1")]
    public class PersonCreatedEvent : SourcedEvent
    {
        public string Name { get; set; }

        public PersonCreatedEvent()
        {
        }

        public PersonCreatedEvent(string name)
        {
            Name = name;
        }
    }
}
