using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.MongoDB.Tests.Fakes
{
    public class FakeEventStream
    {
        [Serializable]
        public class CustomerCreatedEvent : SourcedEvent
        {
            protected CustomerCreatedEvent()
            {
            }

            public CustomerCreatedEvent(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp, string name, int age)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
                Name = name;
                Age = age;
            }

            public string Name
            { get; set; }

            public int Age
            {
                get;
                set;
            }
        }

        [Serializable]
        public class CustomerNameChanged : SourcedEvent
        {
            public Guid CustomerId
            {
                get
                {
                    return EventSourceId;
                }
            }

            public string NewName
            { get; set; }

            protected CustomerNameChanged()
            {

            }

            public CustomerNameChanged(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp, string newName)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
                NewName = newName;
            }
        }

        [Serializable]
        public class AccountNameChangedEvent : EntitySourcedEventBase
        {
            public Guid CustomerId { get; set; }
            public Guid AccountId { get { return EntityId; } }
            public string NewAccountName { get; set; }

            public AccountNameChangedEvent()
            {

            }

            public AccountNameChangedEvent(string newAccountName)
            {
                NewAccountName = newAccountName;
            }
        }

        public static UncommittedEventStream Create()
        {
            var theEventSourceId = Guid.NewGuid();
            var theInitialEventSourceVersion = 0;
            var theCommitId = Guid.NewGuid();
            var theVersion = new Version(1, 0);

            int sequenceCounter = 1;

            var events = new[]
            {
                new UncommittedEvent
                (
                    Guid.NewGuid(), theEventSourceId, sequenceCounter++, theInitialEventSourceVersion, DateTime.Now, 
                    new CustomerCreatedEvent(Guid.NewGuid(), theEventSourceId, sequenceCounter, DateTime.UtcNow, "Foo",35),
                    theVersion
                ),
                new UncommittedEvent
                (
                    Guid.NewGuid(), theEventSourceId, sequenceCounter++, theInitialEventSourceVersion, DateTime.Now, 
                    new CustomerNameChanged(Guid.NewGuid(), theEventSourceId, sequenceCounter, DateTime.UtcNow, "Name"+sequenceCounter),
                    theVersion
                ),
                new UncommittedEvent
                (
                    Guid.NewGuid(), theEventSourceId, sequenceCounter++, theInitialEventSourceVersion, DateTime.Now, 
                    new CustomerNameChanged(Guid.NewGuid(), theEventSourceId, sequenceCounter, DateTime.UtcNow, "Name"+sequenceCounter),
                    theVersion
                ),
                new UncommittedEvent
                (
                    Guid.NewGuid(), theEventSourceId, sequenceCounter++, theInitialEventSourceVersion, DateTime.Now, 
                    new CustomerNameChanged(Guid.NewGuid(), theEventSourceId, sequenceCounter, DateTime.UtcNow, "Name"+sequenceCounter),
                    theVersion
                ),
            };

            var eventStream = new UncommittedEventStream(theCommitId);
            foreach (var e in events) eventStream.Append(e);

            return eventStream;
        }
    }
}
