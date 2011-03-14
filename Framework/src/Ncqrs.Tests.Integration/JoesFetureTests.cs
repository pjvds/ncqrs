using System.IO;
using EventStore;
using EventStore.Serialization;
using Ncqrs.Domain;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.JOliver;
using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;
using NUnit.Framework;

namespace Ncqrs.Tests.Integration
{
    [TestFixture]
    public class JoesFetureTests : FetureTests
    {
        [SetUp]
        public void CopyDatabase()
        {
            File.Copy("NcqrsIntegrationTestsClean.sdf", "NcqrsIntegrationTests.sdf", true);
        }

        protected override void InitializeEnvironment()
        {
            var factory = new AbsoluteOrderingSqlPersistenceFactory("SqlCeEventStore", new BinarySerializer(), false);
            var streamPersister = factory.Build();
            streamPersister.Initialize();
            var store = new OptimisticEventStore(streamPersister, new NullDispatcher());
            var snapshotStore = new JoesSnapshotStoreAdapter(streamPersister);
            NcqrsEnvironment.SetDefault<ISnapshotStore>(snapshotStore);
            var uowFactory = new JoesUnitOfWorkFactory(store);
            NcqrsEnvironment.SetDefault<IUnitOfWorkFactory>(uowFactory);
        }

    }
}