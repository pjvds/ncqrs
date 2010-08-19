using System;
using System.Data.SQLite;
using System.IO;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Storage.SQLite.Tests.Fakes;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Eventing.Storage.SQLite.Tests
{
    [TestFixture]
    public class ManualSQLiteContextTests
    {
        [SetUp]
        public void Setup()
        {
            _path = Path.GetTempFileName();
            _connString = string.Format("Data Source={0};", _path);
            File.Delete(_path);
            SQLiteEventStore.EnsureDatabaseExists(_connString);
            _connection = new SQLiteConnection(_connString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            _context = new ManualSQLiteContext();
            _context.SetContext(_connection, _transaction);
            _store = new SQLiteEventStore(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _transaction.Dispose();
            _connection.Dispose();
            File.Delete(_path);
        }

        private string _path;
        private string _connString;
        private SQLiteEventStore _store;
        private ManualSQLiteContext _context;
        private SQLiteConnection _connection;
        private SQLiteTransaction _transaction;

        [Test]
        public void Save_SmokeTest()
        {
            var sequenceCounter = 0;
            var id = Guid.NewGuid();
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
            _transaction.Commit();
        }
        
        [Test]
        public void Rolling_back_transaction_should_remove_inserted_rows()
        {
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
            _transaction.Rollback();

            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand("SELECT [TimeStamp] FROM [Events]", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Assert.Fail("No rows should exist");
                    }
                }
            }
        }
    }
}
