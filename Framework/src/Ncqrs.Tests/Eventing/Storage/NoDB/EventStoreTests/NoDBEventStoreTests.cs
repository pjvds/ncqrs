using System;
using System.IO;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.NoDB.Tests.Fakes;
using Ncqrs.Spec;
using Rhino.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.EventStoreTests
{
    public abstract class NoDBEventStoreTestFixture: IDisposable
    {
        protected NoDBEventStore EventStore;
        protected object[] Events;
        protected Guid EventSourceId;

        public NoDBEventStoreTestFixture()
        {
            EventStore = new NoDBEventStore("./NoDBTests/"+GetType().Name);
            EventSourceId = Guid.NewGuid();
            Guid entityId = Guid.NewGuid();
            Events = new object[] {new AccountTitleChangedEvent("Title")};
            var eventStream = Prepare.Events(Events)
                .ForSourceUncomitted(EventSourceId, Guid.NewGuid());
            EventStore.Store(eventStream);
        }

        protected string GetPath()
        {
            return "./NoDBTests/" + GetType().Name+"/"+EventSourceId.ToString().Substring(0, 2);
        }

        public void Dispose()
        {
            Directory.Delete(GetPath(), true);
        }
    }
}