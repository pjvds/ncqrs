using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    [EventName("MyCompany:AwesomeApp:Events:NameChanged")]
    [EventNameAlias("MyCompany:AwesomeApp:NameChangedEventttt")]
    public class NameChangedEvent : SourcedEvent
    {
        public string Name { get; set; }

        public NameChangedEvent()
        {
            EventVersion = new Version(2, 0);
        }

        public NameChangedEvent(string name) : this()
        {
            Name = name;
        }
    }
}
