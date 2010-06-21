using System;
using Ncqrs.Eventing.Sourcing;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
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
