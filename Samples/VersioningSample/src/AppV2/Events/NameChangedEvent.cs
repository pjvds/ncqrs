using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    [EventName("MyCompany:AwesomeApp:Events:NameChanged@1")]
    public class NameChangedEvent : SourcedEvent
    {
        public string Forename { get; set; }
        public string Surname { get; set; }

        public NameChangedEvent()
        {
            EventVersion = new Version(1, 1);
        }

        public NameChangedEvent(string forename, string surname) : this()
        {
            Forename = forename;
            Surname = surname;
        }
    }
}
