using System;
using Ncqrs.Eventing.Sourcing;

namespace AwesomeAppRefactored.Events
{
    [Serializable]
    public class NameChangedEvent : SourcedEvent
    {
        public string Name { get; private set; }

        public NameChangedEvent()
        {
        }

        public NameChangedEvent(string name)
        {
            Name = name;
        }
    }
}
