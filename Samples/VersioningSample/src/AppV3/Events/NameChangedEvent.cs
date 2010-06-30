using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    [EventName("MyCompany:AwesomeApp:Events:NameChanged@2")]
    [EventNameAlias("MyCompany:AwesomeApp:Events:NameChanged@1")]
    public class NameChangedEvent : SourcedEvent
    {
        public string Name { get; set; }

        public NameChangedEvent()
        {
        }

        public NameChangedEvent(string name)
        {
            Name = name;
        }
    }
}
