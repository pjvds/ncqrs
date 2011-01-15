using System;
using System.IO;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.NoDB.Tests.Fakes;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    [Category("Integration")]
    public abstract class NoDBEventStoreTestFixture
    {
        protected NoDBEventStore EventStore;
        protected SourcedEvent[] Events;
        protected IEventSource Source;

        [TestFixtureSetUp]
        public void BaseSetup()
        {
            EventStore = new NoDBEventStore("./");
            Source = MockRepository.GenerateMock<IEventSource>();
            Guid aggregateId = Guid.NewGuid();
            Guid entityId = Guid.NewGuid();
            int sequenceCounter = 1;
            Events = new SourcedEvent[]
                         {
                             //new CustomerCreatedEvent(Guid.NewGuid(), aggregateId, sequenceCounter++, DateTime.UtcNow, "Foo",
                             //                         35),
                             //new CustomerNameChanged(Guid.NewGuid(), aggregateId, sequenceCounter++, DateTime.UtcNow,
                             //                        "Name" + sequenceCounter),
                             //new CustomerNameChanged(Guid.NewGuid(), aggregateId, sequenceCounter++, DateTime.UtcNow,
                             //                        "Name" + sequenceCounter),
                             new AccountTitleChangedEvent(Guid.NewGuid(), aggregateId, entityId, sequenceCounter++, DateTime.UtcNow, "Title" + sequenceCounter )
                         };
            Source.Stub(e => e.EventSourceId).Return(aggregateId);
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
}