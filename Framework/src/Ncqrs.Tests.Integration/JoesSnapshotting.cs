using System.IO;
using EventStore.Serialization;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.JOliver;
using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;
using NUnit.Framework;

namespace Ncqrs.Tests.Integration
{
    [TestFixture]
    public class JoesSnapshotting : Snapshotting
    {
        [SetUp]
        public void CopyDatabase()
        {
            File.Copy("NcqrsIntegrationTestsClean.sdf", "NcqrsIntegrationTests.sdf", true);
        }

        protected override IEventStore BuildEventStore()
        {
            var factory = new AbsoluteOrderingSqlPersistenceFactory("SqlCeEventStore", new BinarySerializer());
            var streamPersister = factory.Build();
            streamPersister.Initialize();
            var store = new JoesEventStoreAdapter(streamPersister);
            NcqrsEnvironment.SetDefault<ISnapshotStore>(store);
            return store;
        }
    }
}