namespace Ncqrs.Eventing.Storage.SQLite.Tests.Fakes{
    using System;
    using Domain;

    [Serializable]
    public class CustomerCreatedEvent : DomainEvent{
        public CustomerCreatedEvent(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp, string name, int age) 
            : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
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
                AggregateRootId.Equals(other.AggregateRootId) && 
                EventSequence.Equals(other.EventSequence) && 
                Name.Equals(other.Name) && 
                Age.Equals(other.Age);
            return result;
        }
    }
}