using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.SQLite.Tests{
    using System;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using Fakes;
    using FluentAssertions;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class SQLiteEventStoreTests{
        [SetUp]
        public void Setup()
        {
            _path = Path.GetTempFileName();
            _connString = string.Format("Data Source={0};", _path);
            File.Delete(_path);
            SQLiteEventStore.EnsureDatabaseExists(_connString);
            _store = new SQLiteEventStore(_connString);
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(_path);
        }

        private string _path;
        private string _connString;
        private SQLiteEventStore _store;

        [Test]
        public void EnsuresDatabaseExists()
        {
            using (var conn = new SQLiteConnection(_connString)) conn.Open();
        }

        [Test]
        public void Save_SmokeTest()
        {
            var sequenceCounter = 0;
            var id=Guid.NewGuid();
            var stream = new UncommittedEventStream(Guid.NewGuid());
            stream.Append(
                new UncommittedEvent(Guid.NewGuid(), id, sequenceCounter++, 0, DateTime.UtcNow, new CustomerCreatedEvent("Foo", 35),
                                     new Version(1, 0)));
            stream.Append(
                new UncommittedEvent(Guid.NewGuid(), id, sequenceCounter++, 0, DateTime.UtcNow,
                                     new CustomerNameChanged("Name" + sequenceCounter), new Version(1, 0)));
            stream.Append(
                new UncommittedEvent(Guid.NewGuid(), id, sequenceCounter++, 0, DateTime.UtcNow,
                                     new CustomerNameChanged("Name" + sequenceCounter), new Version(1, 0)));
                        
            _store.Store(stream);
        }

        [Test]
        public void Retrieving_all_events_should_return_the_same_as_added() {
            var id=Guid.NewGuid();
            var sequenceCounter=0;

            var stream = new UncommittedEventStream(Guid.NewGuid());
            stream.Append(
                new UncommittedEvent(Guid.NewGuid(), id, sequenceCounter++, 0, DateTime.UtcNow, new CustomerCreatedEvent("Foo", 35),
                                     new Version(1, 0)));
            stream.Append(
                new UncommittedEvent(Guid.NewGuid(), id, sequenceCounter++, 0, DateTime.UtcNow,
                                     new CustomerNameChanged("Name" + sequenceCounter), new Version(1, 0)));
            stream.Append(
                new UncommittedEvent(Guid.NewGuid(), id, sequenceCounter++, 0, DateTime.UtcNow,
                                     new CustomerNameChanged("Name" + sequenceCounter), new Version(1, 0)));
            
            _store.Store(stream);

            var result=_store.ReadFrom(id, long.MinValue, long.MaxValue);
            result.Count().Should().Be(stream.Count());
            result.First().EventIdentifier.Should().Be(stream.First().EventIdentifier);
            //TODO:

            var streamList = stream.ToList();
            var resultList = result.ToList();

            for (int i = 0; i < resultList.Count; i++)
            {
                Assert.IsTrue(AreEqual(streamList[i], resultList[i]));
            }
        }

        private static bool AreEqual(UncommittedEvent uncommitted, CommittedEvent committed)
        {
            return uncommitted.EventIdentifier == committed.EventIdentifier
                   && uncommitted.EventSourceId == committed.EventSourceId
                   && uncommitted.Payload.Equals(committed.Payload)
                   && uncommitted.EventTimeStamp == committed.EventTimeStamp
                   && uncommitted.EventSequence == committed.EventSequence;
        }

        [Test]
        public void Retrieved_event_should_having_identical_timestamp_as_persisted() {
            var id = Guid.NewGuid();
            var utcNow = DateTime.UtcNow.Date.AddHours(9).AddTicks(-1);

            var stream = new UncommittedEventStream(Guid.NewGuid());
            stream.Append(
                new UncommittedEvent(Guid.NewGuid(), id, 1, 0, utcNow, new CustomerCreatedEvent("Foo", 35),
                                     new Version(1, 0)));
            
            _store.Store(stream);

            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand("SELECT [TimeStamp] FROM [Events]", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timestamp = (long)reader["Timestamp"];
                        timestamp.Should().Be(utcNow.Ticks);
                    }
                }
            }
        }
    }
}