namespace Ncqrs.Eventing.Storage.SQLite.Tests.Fakes{
    using System;
    using Domain;
    using Ncqrs.Eventing.Sourcing;

    [Serializable]
    public class CustomerNameChanged : SourcedEvent{
        public CustomerNameChanged(Guid eventIdentifier, Guid eventSourceId, long eventSequence, DateTime eventTimeStamp, string newName)
            : base(eventIdentifier, eventSourceId, eventSequence, eventTimeStamp)
        {
            NewName = newName;
        }

        public Guid CustomerId { get { return EventSourceId; } }

        public string NewName { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CustomerNameChanged;
            if (other == null) return false;
            var result = EventIdentifier.Equals(other.EventIdentifier) &&
                EventSourceId.Equals(other.EventSourceId) && 
                EventSequence.Equals(other.EventSequence) && 
                CustomerId.Equals(other.CustomerId) && 
                NewName.Equals(other.NewName);
            return result;
        }
    }
}