using System;
using FluentAssertions;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.RavenDB.Tests
{
    [TestFixture]
    public class RavenDBSnapshotStoreTests : RavenDBTestBase
    {
        [Serializable]
        public class MySnapshot
        {
            public string Value { get; set; }
        }

        [Test]
        public void Saving_snapshot_should_not_throw_an_exception_when_snapshot_is_valid()
        {
            var targetStore = new RavenDBSnapshotStore(_documentStore);

            var anId = Guid.NewGuid();
            var aVersion = 12;
            var snapshot = new Snapshot(anId, aVersion, new MySnapshot {Value = "Some value"});

            targetStore.SaveSnapshot(snapshot);

            var savedSnapshot = targetStore.GetSnapshot(anId, long.MaxValue);
            savedSnapshot.EventSourceId.Should().Be(anId);
            savedSnapshot.Version.Should().Be(aVersion);
            ((MySnapshot) savedSnapshot.Payload).Value.Should().Be("Some value");
        }
    }
}