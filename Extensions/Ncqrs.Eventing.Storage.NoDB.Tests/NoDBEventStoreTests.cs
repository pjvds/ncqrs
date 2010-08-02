using System;
using System.IO;
using System.Threading;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.NoDB.Tests.Fakes;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Eventing.Storage.NoDB.Tests
{
    public class NoDBEventStoreTestFixture
    {
        protected NoDBEventStore EventStore;
        protected SourcedEvent[] Events;
        protected IEventSource Source;

        [TestFixtureSetUp]
        public void BaseSetup()
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
            EventStore.Save(Source);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Directory.Delete(Source.EventSourceId.ToString().Substring(0, 2), true);
        }
    }

    public class when_saving_events_based_on_stale_state : NoDBEventStoreTestFixture
    {
        [Test, ExpectedException(typeof(ConcurrencyException))]
        public void it_should_throw_a_concurrency_exception() { EventStore.Save(Source); }
    }
}