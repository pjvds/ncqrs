namespace Ncqrs.Eventing.Storage.SQLite.Tests.Fakes{
    using System;
    using Domain;

    [Serializable]
    public class CustomerNameChanged : DomainEvent{
        public CustomerNameChanged(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp, string newName) 
            : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
        {
            NewName = newName;
        }

        public Guid CustomerId { get { return AggregateRootId; } }

        public string NewName { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CustomerNameChanged;
            if (other == null) return false;
            var result = EventIdentifier.Equals(other.EventIdentifier) && 
                AggregateRootId.Equals(other.AggregateRootId) && 
                EventSequence.Equals(other.EventSequence) && 
                CustomerId.Equals(other.CustomerId) && 
                NewName.Equals(other.NewName);
            return result;
        }
    }
}