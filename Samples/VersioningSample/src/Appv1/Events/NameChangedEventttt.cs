using System;
using Ncqrs.Eventing.Sourcing;

namespace AwesomeApp.Events
{
    [Serializable]
    public class NameChangedEventttt : SourcedEvent
    {
        public string Forename { get; private set; }
        public string Surname { get; private set; }

        public NameChangedEventttt()
        {
        }

        public NameChangedEventttt(string forename, string surname)
        {
            Forename = forename;
            Surname = surname;
        }
    }
}
