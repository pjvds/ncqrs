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
            var events = new SourcedEvent[]{
                new CustomerCreatedEvent(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Foo", 35), 
                new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Name" + sequenceCounter), 
                new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Name" + sequenceCounter)
            };
            var source = MockRepository.GenerateMock<IEventSource>();
            source.Stub(e => e.EventSourceId).Return(id);
            source.Stub(e => e.InitialVersion).Return(0);
            source.Stub(e => e.Version).Return(events.Length);
            source.Stub(e => e.GetUncommittedEvents()).Return(events);
            _store.Save(source);
        }

        [Test]
        public void Retrieving_all_events_should_return_the_same_as_added() {
            var id=Guid.NewGuid();
            var sequenceCounter=0;
            var events=new SourcedEvent[]
                             {
                                 new CustomerCreatedEvent(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow, "Foo",35),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,"Name" + sequenceCounter),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,"Name" + sequenceCounter),
                                 new CustomerNameChanged(Guid.NewGuid(), id, sequenceCounter++, DateTime.UtcNow,"Name" + sequenceCounter)
                             };

            var eventSource=MockRepository.GenerateMock<IEventSource>();
            eventSource.Stub(e => e.EventSourceId).Return(id);
            eventSource.Stub(e => e.InitialVersion).Return(0);
            eventSource.Stub(e => e.Version).Return(events.Length);
            eventSource.Stub(e => e.GetUncommittedEvents()).Return(events);
            _store.Save(eventSource);
            
            var result=_store.GetAllEvents(id);
            result.Count().Should().Be(events.Length);
            result.First().EventIdentifier.Should().Be(events.First().EventIdentifier);
            Assert.IsTrue(result.SequenceEqual(events));
        }

        [Test]
        public void Retrieved_event_should_having_identical_timestamp_as_persisted() {
            var id = Guid.NewGuid();
            var utcNow = DateTime.UtcNow.Date.AddHours(9).AddTicks(-1);
            
            var events = new SourcedEvent[]
            {
                new CustomerCreatedEvent(Guid.NewGuid(), id, 0, utcNow, "Foo", 35)
            };

            var eventSource = MockRepository.GenerateMock<IEventSource>();
            eventSource.Stub(e => e.EventSourceId).Return(id);
            eventSource.Stub(e => e.InitialVersion).Return(0);
            eventSource.Stub(e => e.Version).Return(events.Length);
            eventSource.Stub(e => e.GetUncommittedEvents()).Return(events);
            _store.Save(eventSource);

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