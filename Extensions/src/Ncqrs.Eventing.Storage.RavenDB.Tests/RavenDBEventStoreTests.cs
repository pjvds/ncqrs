using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Ncqrs.Domain;
using NUnit.Framework;
using Raven.Client;
using Rhino.Mocks;

namespace Ncqrs.Eventing.Storage.RavenDB.Tests
{
    [TestFixture]
    public class RavenDBEventStoreTests : RavenDBTestBase
    {
        [Serializable]
        public class CustomerCreatedEvent : DomainEvent
        {
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

            public override bool Equals(object obj)
            {
                var other = obj as CustomerCreatedEvent;
                if (other == null)
                {
                    return false;
                }
                bool result = EventIdentifier.Equals(other.EventIdentifier)
                       && AggregateRootId.Equals(other.AggregateRootId)
                       && EventSequence.Equals(other.EventSequence)
                       //&& EventTimeStamp.Equals(other.EventTimeStamp)
                       && Name.Equals(other.Name)
                       && Age.Equals(other.Age);
                return result;
            }
        }

        [Serializable]
        public class CustomerNameChanged : DomainEvent
        {
            public Guid CustomerId
            {
                get
                {
                    return AggregateRootId;
                }
            }

            public string NewName
            { get; set; }

            public CustomerNameChanged(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp, string newName)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
                NewName = newName;
            }

            public override bool Equals(object obj)
            {
                var other = obj as CustomerNameChanged;
                if (other == null)
                {
                    return false;
                }
                bool result = EventIdentifier.Equals(other.EventIdentifier)
                       && AggregateRootId.Equals(other.AggregateRootId)
                       && EventSequence.Equals(other.EventSequence)
                       //&& EventTimeStamp.Equals(other.EventTimeStamp)
                       && CustomerId.Equals(other.CustomerId)
                       && NewName.Equals(other.NewName);
                return result;
            }
        }

        

        [Test]
        public void Saving_event_source_should_succeed()
        {
            var targetStore = new RavenDBEventStore(_documentStore);
            var id = Guid.NewGuid();

            int sequenceCounter = 0;

            var events = new ISourcedEvent[]
                             {
                                 new CustomerCreatedEvent(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Foo",
                                                          35),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,
                                                         "Name" + sequenceCounter),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,
                                                         "Name" + sequenceCounter),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,
                                                         "Name" + sequenceCounter)
                             };

            var eventSource = MockRepository.GenerateMock<IEventSource>();
            eventSource.Stub(e => e.Id).Return(id);
            eventSource.Stub(e => e.InitialVersion).Return(0);
            eventSource.Stub(e => e.Version).Return(events.Length);
            eventSource.Stub(e => e.GetUncommittedEvents()).Return(events);

            targetStore.Save(eventSource);
        }

        [Test]
        public void Saving_event_source_while_there_is_a_newer_event_source_should_throw_concurency_exception()
        {
            var targetStore = new RavenDBEventStore(_documentStore);
            var id = Guid.NewGuid();

            int sequenceCounter = 0;

            var events = new ISourcedEvent[]
                             {
                                 new CustomerCreatedEvent(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Foo",
                                                          35),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,
                                                         "Name" + sequenceCounter)
                             };
            
            var eventSource = MockRepository.GenerateMock<IEventSource>();
            eventSource.Stub(e => e.Id).Return(id).Repeat.Any();
            eventSource.Stub(e => e.InitialVersion).Return(0).Repeat.Any();
            eventSource.Stub(e => e.Version).Return(events.Length).Repeat.Any();
            eventSource.Stub(e => e.GetUncommittedEvents()).Return(events).Repeat.Any();

            targetStore.Save(eventSource);
            
            Action act = () => targetStore.Save(eventSource);
            act.ShouldThrow<ConcurrencyException>();
        }

        [Test]
        public void Retrieving_all_events_should_return_the_same_as_added()
        {
            var targetStore = new RavenDBEventStore(_documentStore);
            var id = Guid.NewGuid();

            int sequenceCounter = 0;

            var events = new ISourcedEvent[]
                             {
                                 new CustomerCreatedEvent(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Foo",
                                                          35),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,
                                                         "Name" + sequenceCounter),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,
                                                         "Name" + sequenceCounter),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,
                                                         "Name" + sequenceCounter)
                             };

            var eventSource = MockRepository.GenerateMock<IEventSource>();
            eventSource.Stub(e => e.Id).Return(id);
            eventSource.Stub(e => e.InitialVersion).Return(0);
            eventSource.Stub(e => e.Version).Return(events.Length);
            eventSource.Stub(e => e.GetUncommittedEvents()).Return(events);

            targetStore.Save(eventSource);

            var result = targetStore.GetAllEvents(id);
            result.Count().Should().Be(events.Length);
            result.First().EventIdentifier.Should().Be(events.First().EventIdentifier);
            Assert.IsTrue(result.SequenceEqual(events));
        }        
    }
}
