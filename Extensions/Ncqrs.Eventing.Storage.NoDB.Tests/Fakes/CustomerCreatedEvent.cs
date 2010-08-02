using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.Fakes
{
    [Serializable]
    public class CustomerCreatedEvent : SourcedEvent
    {
        public CustomerCreatedEvent()
        {
        }

        public CustomerCreatedEvent(Guid eventIdentifier, Guid eventSourceId, long eventSequence,
                                    DateTime eventTimeStamp, string name, int age)
            : base(eventIdentifier, eventSourceId, eventSequence, eventTimeStamp)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; set; }

        public int Age { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CustomerCreatedEvent;
            if (other == null) return false;
            bool result = EventIdentifier.Equals(other.EventIdentifier) &&
                          EventSourceId.Equals(other.EventSourceId) &&
                          EventSequence.Equals(other.EventSequence) &&
                          EventTimeStamp.Equals(other.EventTimeStamp) &&
                          Name.Equals(other.Name) &&
                          Age.Equals(other.Age);
            return result;
        }
    }
}