using System;
using System.IO;
using FluentAssertions;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Storage.NoDB.Tests.Fakes;
using Newtonsoft.Json;
using Xunit;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.SnapshotStoreTests
{
    //[Ignore("")]
    public class when_saving_a_new_snapshot : IClassFixture<when_saving_a_new_snapshotFixture>
    {
        private when_saving_a_new_snapshotFixture _fixture;

        public when_saving_a_new_snapshot(when_saving_a_new_snapshotFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void it_should_create_the_snapshot_file()
        {
            Assert.True(File.Exists(Path.Combine(_fixture.FolderName, _fixture.FileName)));
        }

        [Fact]
        public void it_should_write_the_snapshot_to_the_snapshot_file()
        {
            using (var reader = new StreamReader(File.Open(Path.Combine(_fixture.FolderName, _fixture.FileName), FileMode.Open)))
            {
                reader.ReadLine(); //Throw out type line
                var jsonSerializer = JsonSerializer.Create(null);
                var snapshot = jsonSerializer.Deserialize<Snapshot>(new JsonTextReader(reader));
                snapshot.EventSourceId.Should().Be(_fixture.Snapshot.EventSourceId);
                snapshot.Version.Should().Be(_fixture.Snapshot.Version);
            }

        }
    }

    public class when_saving_a_new_snapshotFixture : NoDBSnapshotStoreTestFixture
    {
        public string FolderName;
        public string FileName;

        public when_saving_a_new_snapshotFixture(): base()
        {
            Snapshot = new Snapshot(Guid.NewGuid(), 1, new TestSnapshot { Name = "TestName" });
            FolderName = Snapshot.EventSourceId.ToString().Substring(0, 2);
            FileName = Snapshot.EventSourceId.ToString().Substring(2) + ".ss";
            SnapshotStore.SaveSnapshot(Snapshot);
        }
    }
}