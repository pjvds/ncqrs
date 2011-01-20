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
            _transaction.Commit();
        }
        
        [Test]
        public void Rolling_back_transaction_should_remove_inserted_rows()
        {
            var id = Guid.NewGuid();
            var utcNow = DateTime.UtcNow.Date.AddHours(9).AddTicks(-1);

            var stream = new UncommittedEventStream(Guid.NewGuid());
            stream.Append(
                new UncommittedEvent(Guid.NewGuid(), id, 1, 0, utcNow, new CustomerCreatedEvent("Foo", 35),
                                     new Version(1, 0)));

            _store.Store(stream);
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
