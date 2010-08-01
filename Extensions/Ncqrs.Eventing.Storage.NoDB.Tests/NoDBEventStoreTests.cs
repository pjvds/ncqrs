using System;
using System.Collections.Generic;
using System.Linq;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.NoDB.Tests.Fakes;
using Ncqrs.Eventing.Storage.Serialization;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Eventing.Storage.NoDB.Tests
{
    public class NoDBEventStoreTestFixture
    {
        protected NoDBEventStore EventStore;
        protected IEventSource Source;
        protected SourcedEvent[] Events;

        [SetUp]
        public void Setup()
        {
            EventStore = new NoDBEventStore("./");
            Source = MockRepository.GenerateMock<IEventSource>();
            Guid id = Guid.NewGuid();
            int sequenceCounter = 0;
            Events = new SourcedEvent[]
                             {
                                 new CustomerCreatedEvent(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Foo",
                                                          35),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,
                                                         "Name" + sequenceCounter),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,
                                                         "Name" + sequenceCounter)
                             };
            Source.Stub(e => e.EventSourceId).Return(id);
            Source.Stub(e => e.InitialVersion).Return(0);
            Source.Stub(e => e.Version).Return(Events.Length);
            Source.Stub(e => e.GetUncommittedEvents()).Return(Events);
        }
    }

    public class when_getting_all_events_for_an_event_source : NoDBEventStoreTestFixture
    {
        private SourcedEvent[] _returnedEvents;

        [SetUp]
        public void SetUp()
        {
            EventStore.Save(Source);
            _returnedEvents = EventStore.GetAllEvents(Source.EventSourceId).ToArray();
        }

        [Test]
        public void it_should_get_the_exact_same_events_that_were_committed()
        {
            Assert.That(_returnedEvents, Is.EqualTo(Events));
        }
    }
}