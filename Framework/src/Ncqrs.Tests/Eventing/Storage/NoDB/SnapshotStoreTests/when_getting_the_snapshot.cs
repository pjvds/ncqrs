using System;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using NUnit.Framework;
using FluentAssertions;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.SnapshotStoreTests
{
    public class when_getting_the_snapshot : NoDBSnapshotStoreTestFixture
    {
        private Snapshot _retrieved;

        [TestFixtureSetUp]
        public void SetUp()
        {
            Snapshot = new Snapshot(Guid.NewGuid(), 1, new TestSnapshot { Name = "Name" });
            SnapshotStore.SaveSnapshot(Snapshot);
            _retrieved = SnapshotStore.GetSnapshot(Snapshot.EventSourceId, long.MaxValue);
        }

        [Test]
        public void it_should_get_the_last_snapshot_saved()
        {
            _retrieved.EventSourceId.Should().Be(Snapshot.EventSourceId);
            _retrieved.Version.Should().Be(Snapshot.Version);
        }

        [Test]
        public void it_should_return_null_if_the_snapshot_does_not_exist()
        {
            var shouldbenull = SnapshotStore.GetSnapshot(Guid.NewGuid(), long.MaxValue);
            Assert.That(shouldbenull, Is.Null);
        }
    }
}