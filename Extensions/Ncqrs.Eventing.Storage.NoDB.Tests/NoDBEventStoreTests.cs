using System;
using System.IO;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.NoDB.Tests.Fakes;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Eventing.Storage.NoDB.Tests
{
    public class NoDBEventStoreTestFixture
    {
        protected NoDBEventStore EventStore;
        protected IEventSource Source;

        [SetUp]
        public void Setup()
        {
            EventStore = new NoDBEventStore("./");
            Source = MockRepository.GenerateMock<IEventSource>();
            Guid id = Guid.NewGuid();
            int sequenceCounter = 0;
            var events = new SourcedEvent[]
                             {
                                 new CustomerCreatedEvent(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Foo",35),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Name" + sequenceCounter),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Name" + sequenceCounter)
                             };
            Source.Stub(e => e.EventSourceId).Return(id);
            Source.Stub(e => e.InitialVersion).Return(0);
            Source.Stub(e => e.Version).Return(events.Length);
            Source.Stub(e => e.GetUncommittedEvents()).Return(events);
        }
    }

    public class when_saving_a_new_event_source : NoDBEventStoreTestFixture
    {
        [SetUp]
        public void SetUp()
        {
            EventStore.Save(Source);
        }

        [Test]
        public void it_should_create_a_new_event_history_file()
        {
            var foldername = Source.EventSourceId.ToString().Substring(0, 2);
            var filename = Source.EventSourceId.ToString().Substring(2);
            var file = File.Exists(Path.Combine(foldername, filename));
        }
    }
}