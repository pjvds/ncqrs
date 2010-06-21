using System;
using Ncqrs.Eventing.Sourcing;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    public class NameChangedEvent : SourcedEvent
    {
        public string Forename { get; private set; }
        public string Surname { get; private set; }

        public NameChangedEvent(string forename, string surname)
        {
            Forename = forename;
            Surname = surname;
        }
    }
}
