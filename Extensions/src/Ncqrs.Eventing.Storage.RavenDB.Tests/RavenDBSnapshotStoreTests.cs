using System;
using FluentAssertions;
using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.RavenDB.Tests
{
    [TestFixture]
    public class RavenDBSnapshotStoreTests : RavenDBTestBase
    {
        [Serializable]
        public class MySnapshot : Snapshot
        {
        }

        [Test]
        public void Saving_snapshot_should_not_throw_an_exception_when_snapshot_is_valid()
        {
            var targetStore = new RavenDBSnapshotStore(_documentStore);

            var anId = Guid.NewGuid();
            var aVersion = 12;
            var snapshot = new MySnapshot { EventSourceId = anId, EventSourceVersion = aVersion };

            targetStore.SaveShapshot(snapshot);

            var savedSnapshot = targetStore.GetSnapshot(anId);
            savedSnapshot.EventSourceId.Should().Be(anId);
            savedSnapshot.EventSourceVersion.Should().Be(aVersion);
        }
    }
}