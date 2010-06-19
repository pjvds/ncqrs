using System;
using System.Linq;
using FluentAssertions;
using Ncqrs.Eventing.Sourcing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Eventing.Storage.WindowsAzure.Tests
{
    [TestFixture]
    public class AzureEventStoreTests
    {
        [Serializable]
        public class FooEvent : SourcedEvent
        {
            public Guid EventIdentifier { get; private set; }
            public DateTime EventTimeStamp { get; private set; }
            public Guid EventSourceId { get; private set; }
            public long EventSequence { get; private set; }

            public FooEvent(Guid eventSourceId, long eventSequence)
            {
                EventIdentifier = Guid.NewGuid();
                EventTimeStamp = DateTime.UtcNow;
                EventSourceId = eventSourceId;
                EventSequence = eventSequence;
            }
        }

        [Serializable]
        public class BarEvent : SourcedEvent
        {
            public Guid EventIdentifier { get; private set; }
            public DateTime EventTimeStamp { get; private set; }
            public Guid EventSourceId { get; private set; }
            public long EventSequence { get; private set; }

            public BarEvent(Guid eventSourceId, long eventSequence)
            {
                EventIdentifier = Guid.NewGuid();
                EventTimeStamp = DateTime.UtcNow;
                EventSourceId = eventSourceId;
                EventSequence = eventSequence;
            }
        }

        [TearDown]
        public void TearDown()
        {
            var theEventStore = new AzureEventStore();
            theEventStore.ClearStore();
        }

        [Test]
        public void All_the_events_from_a_new_event_source_should_add_events_to_event_store()
        {
            var mock = MockRepository.GenerateMock<IEventSource>();
            mock.Stub(m => mock.EventSourceId).Return(Guid.NewGuid()).Repeat.Any();
            mock.Stub(m => mock.GetUncommittedEvents()).Return(new SourcedEvent[] { new FooEvent(mock.EventSourceId, 1), new BarEvent(mock.EventSourceId, 2) });

            var theEventStore = new AzureEventStore();
            theEventStore.Save(mock);

            var storedEvents = theEventStore.GetAllEvents(mock.EventSourceId);
            storedEvents.Count().Should().Be(2);
        }

        [Test]
        public void All_the_events_from_a_event_source_should_add_events_to_event_store()
        {
            var mock = MockRepository.GenerateStub<IEventSource>();
            mock.Stub(m => mock.EventSourceId).Return(Guid.NewGuid()).Repeat.Any();
            mock.Stub(m => mock.GetUncommittedEvents()).Return(new SourcedEvent[] { new FooEvent(mock.EventSourceId, 1), new BarEvent(mock.EventSourceId, 2) }).Repeat.Once();

            var theEventStore = new AzureEventStore();
            theEventStore.Save(mock);

            mock.Stub(m => mock.Version).Return(2);
            mock.Stub(m => mock.GetUncommittedEvents()).Return(new SourcedEvent[] { new FooEvent(mock.EventSourceId, 3), new BarEvent(mock.EventSourceId, 4) });
            theEventStore.Save(mock);


            var storedEvents = theEventStore.GetAllEvents(mock.EventSourceId);
            storedEvents.Count().Should().Be(4);
        }

        [Test]
        public void Saving_an_old_event_source_should_throw_exception()
        {
            var mock = MockRepository.GenerateStub<IEventSource>();
            mock.Stub(m => mock.EventSourceId).Return(Guid.NewGuid()).Repeat.Any();
            mock.Stub(m => mock.GetUncommittedEvents()).Return(new SourcedEvent[] { new FooEvent(mock.EventSourceId, 1), new BarEvent(mock.EventSourceId, 2) }).Repeat.Once();

            var theEventStore = new AzureEventStore();
            theEventStore.Save(mock);

            mock.Stub(m => mock.Version).Return(1);
            mock.Stub(m => mock.GetUncommittedEvents()).Return(new SourcedEvent[] { new FooEvent(mock.EventSourceId, 3), new BarEvent(mock.EventSourceId, 4) });
            Action act = () => theEventStore.Save(mock);

            act.ShouldThrow<ConcurrencyException>();
        }
    }
}
