using System;
using Ncqrs.Eventing.Sourcing;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    public class PersonCreatedEvent : SourcedEvent
    {
        public string Name { get; private set; }

        public PersonCreatedEvent()
        {
        }

        public PersonCreatedEvent(string name)
        {
            Name = name;
        }
    }
}
