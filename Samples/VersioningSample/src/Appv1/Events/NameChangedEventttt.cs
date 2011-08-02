using System;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage;

namespace AwesomeApp.Events
{
    [Serializable]
    [EventName("MyCompany:AwesomeApp:NameChangedEventttt")]
    public class NameChangedEventttt
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
