using System;
using System.IO;
using Ncqrs.Eventing.Storage.NoDB.Tests.Fakes;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.SnapshotStoreTests
{
    public class when_saving_a_new_snapshot : NoDBSnapshotStoreTestFixture
    {
        private string _foldername;
        private string _filename;

        [TestFixtureSetUp]
        public void SetUp()
        {
            Snapshot = new TestSnapshot { EventSourceId = Guid.NewGuid(), EventSourceVersion = 1, Name = "TestName"};
            _foldername = Snapshot.EventSourceId.ToString().Substring(0, 2);
            _filename = Snapshot.EventSourceId.ToString().Substring(2) + ".ss";
            SnapshotStore.SaveShapshot(Snapshot);
        }

        [Test]
        public void it_should_create_the_snapshot_file()
        {
            Assert.That(File.Exists(Path.Combine(_foldername, _filename)));
        }

        [Test]
        public void it_should_write_the_snapshot_to_the_snapshot_file()
        {
            using (var reader = new StreamReader(File.Open(Path.Combine(_foldername, _filename), FileMode.Open)))
            {
                reader.ReadLine(); //Throw out type line
                var jsonSerializer = JsonSerializer.Create(null);
                var snapshot = jsonSerializer.Deserialize<TestSnapshot>(new JsonTextReader(reader));
                Assert.That(snapshot, Is.EqualTo(Snapshot));
            }

        }
    }
}