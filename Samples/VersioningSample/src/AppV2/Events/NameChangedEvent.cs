using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    [EventName("MyCompany:AwesomeApp:Events:NameChanged")]
    [EventNameAlias("MyCompany:AwesomeApp:NameChangedEventttt")]
    public class NameChangedEvent
    {
        public string Forename { get; set; }
        public string Surname { get; set; }

        public NameChangedEvent()
        {
        }

        public NameChangedEvent(string forename, string surname) : this()
        {
            Forename = forename;
            Surname = surname;
        }
    }
}
