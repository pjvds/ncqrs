using System;
using System.IO;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.NoDB.Tests.Fakes;
using Ncqrs.Spec;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    [Category("Integration")]
    public abstract class NoDBEventStoreTestFixture
    {
        protected NoDBEventStore EventStore;
        protected object[] Events;
        protected Guid EventSourceId;

        [TestFixtureSetUp]
        public void BaseSetup()
        {
            EventStore = new NoDBEventStore("./NoDBTests/"+GetType().Name);
            EventSourceId = Guid.NewGuid();
            Guid entityId = Guid.NewGuid();
            Events = new object[] {new AccountTitleChangedEvent("Title")};
            var eventStream = Prepare.Events(Events)
                .ForSourceUncomitted(EventSourceId, Guid.NewGuid());
            EventStore.Store(eventStream);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Directory.Delete(GetPath(), true);
        }

        protected string GetPath()
        {
            return "./NoDBTests/" + GetType().Name+"/"+EventSourceId.ToString().Substring(0, 2);
        }
    }
}