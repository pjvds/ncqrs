namespace Ncqrs.Eventing.Storage.SQLite.Tests.Fakes{
    using System;
    using Domain;
    using Ncqrs.Eventing.Sourcing;

    [Serializable]
    public class CustomerCreatedEvent : SourcedEvent{
        public CustomerCreatedEvent(Guid eventIdentifier, Guid eventSourceId, long eventSequence, DateTime eventTimeStamp, string name, int age)
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
            var result = EventIdentifier.Equals(other.EventIdentifier) &&
                EventSourceId.Equals(other.EventSourceId) && 
                EventSequence.Equals(other.EventSequence) && 
                Name.Equals(other.Name) && 
                Age.Equals(other.Age);
            return result;
        }
    }
}