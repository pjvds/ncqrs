using System;
using System.IO;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Ncqrs.Eventing.Storage.NoDB.Tests.SnapshotStoreTests
{
    public class NoDBSnapshotStoreTestFixture
    {
        protected NoDBSnapshotStore SnapshotStore;
        protected Snapshot Snapshot;

        [TestFixtureSetUp]
        public void BaseSetup()
        {
            SnapshotStore = new NoDBSnapshotStore("");
        }

        [TestFixtureTearDown]
        public void BaseTearDown()
        {
            if (Snapshot != null)
            {
                var foldername = Snapshot.EventSourceId.ToString().Substring(0, 2);
                if (Directory.Exists(foldername))
                    Directory.Delete(foldername, true);
            }

        }
    }

    public class TestSnapshot
    {
        public string Name   { get; set; }

        public bool Equals(TestSnapshot other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TestSnapshot)) return false;
            return Equals((TestSnapshot) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}