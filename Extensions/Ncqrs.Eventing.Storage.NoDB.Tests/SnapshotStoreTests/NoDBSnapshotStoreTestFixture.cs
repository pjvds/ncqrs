using System;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.SnapshotStoreTests
{
    public class NoDBSnapshotStoreTestFixture
    {
        protected NoDBSnapshotStore SnapshotStore;

        [TestFixtureSetUp]
        public void BaseSetup()
        {
            SnapshotStore = new NoDBSnapshotStore("");
        }
    }

    public class NoDBSnapshotStore : ISnapshotStore
    {
        private readonly string _path;

        public NoDBSnapshotStore(string path)
        {
            _path = path;
        }

        public void SaveShapshot(ISnapshot source)
        {
            
        }

        public ISnapshot GetSnapshot(Guid eventSourceId)
        {
            throw new NotImplementedException();
        }
    }

    public class when_saving_a_new_snapshot : NoDBSnapshotStoreTestFixture
    {
        [SetUp]
        public void SetUp()
        {
            var snapshot = new TestSnapshot();
            SnapshotStore.SaveShapshot(snapshot);
        }

        [Test]
        public void it_should_write_the_snapshot_to_the_snapshot_file()
        {
            
        }
    }

    public class TestSnapshot : Snapshot
    {
    }
}