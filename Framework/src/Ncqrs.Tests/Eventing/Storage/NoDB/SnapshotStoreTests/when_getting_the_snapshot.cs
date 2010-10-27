using System;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.SnapshotStoreTests
{
    public class when_getting_the_snapshot : NoDBSnapshotStoreTestFixture
    {
        private ISnapshot _retrieved;

        [TestFixtureSetUp]
        public void SetUp()
        {
            Snapshot = new TestSnapshot { EventSourceId = Guid.NewGuid(), EventSourceVersion = 1, Name = "TestName" };
            SnapshotStore.SaveShapshot(Snapshot);
            _retrieved = SnapshotStore.GetSnapshot(Snapshot.EventSourceId);
        }

        [Test]
        public void it_should_get_the_last_snapshot_saved()
        {
            Assert.That(_retrieved, Is.EqualTo(Snapshot));
        }

        [Test]
        public void it_should_return_null_if_the_snapshot_does_not_exist()
        {
            var shouldbenull = SnapshotStore.GetSnapshot(Guid.NewGuid());
            Assert.That(shouldbenull, Is.Null);
        }
    }
}