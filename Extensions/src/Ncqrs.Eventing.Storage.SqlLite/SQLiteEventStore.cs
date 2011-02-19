using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.SQLite
{
    public class SQLiteEventStore : IEventStore
    {
        private IPropertyBagConverter _converter;
        private ISQLiteContext _context;

        public SQLiteEventStore(string connectionString) : this(new DefaultSQLiteContext(connectionString))
        {
        }

        public SQLiteEventStore(string connectionString, IPropertyBagConverter converter) : this(new DefaultSQLiteContext(connectionString), converter)
        {
        }

        public SQLiteEventStore(ISQLiteContext context) : this(context, new PropertyBagConverter())
        {
        }

        public SQLiteEventStore(ISQLiteContext context, IPropertyBagConverter converter)
        {
            _context = context;
            _converter = converter;
        }
        

        public CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion)
        {
            var results = new List<CommittedEvent>();
            _context.WithConnection(connection =>
            {
                using (var cmd = new SQLiteCommand(Query.SelectAllEventsFromQuery, connection))
                {
                    cmd.AddParam("EventSourceId", id)
                        .AddParam("EventSourceMinVersion", minVersion)
                        .AddParam("EventSourceMaxVersion", maxVersion);
                    results.AddRange(ReadEvents(cmd, id));
                }
            });
            return new CommittedEventStream(id, results);
        }

        private IEnumerable<CommittedEvent> ReadEvents(SQLiteCommand cmd, Guid id)
        {
            using (var reader = cmd.ExecuteReader())
            {
                var formatter = new BinaryFormatter();
                while (reader.Read())
                {
                    var rawData = (Byte[])reader["Data"];

                    using (var dataStream = new MemoryStream(rawData))
                    {
                        var bag = (PropertyBag)formatter.Deserialize(dataStream);
                        var evnt = _converter.Convert(bag);
                        var dummyCommitId = Guid.Empty;
                        var sequence = (long) reader["Sequence"];
                        var timeStamp = (long)reader["TimeStamp"];
                        var eventId = (Guid)reader["EventId"];
                        yield return new CommittedEvent(dummyCommitId, eventId, id, sequence, new DateTime(timeStamp), evnt, /*TODO*/ new Version(1,0));
                    }
                }
            }
        }

        public void Store(UncommittedEventStream events)
        {            
            _context.WithConnection(connection => 
            _context.WithTransaction(connection, transaction =>
            {
                SaveEventSources(events, transaction);
                SaveEvents(events, transaction);
                UpdateEventSources(events, transaction);
            }));    
        }

        private static void SaveEventSources(UncommittedEventStream events, SQLiteTransaction transaction)
        {
            var eventSources = events.Sources;
            foreach (var eventSource in eventSources)
            {
                var currentVersion = GetVersion(events.SourceId, transaction);
                if (currentVersion == null)
                {
                    AddEventSource(eventSource, transaction);
                }
                else if (currentVersion.Value != eventSource.InitialVersion)
                {
                    throw new ConcurrencyException(events.SourceId, eventSource.InitialVersion);
                }
            }
            
        }

        private static int? GetVersion(Guid providerId, SQLiteTransaction transaction)
        {
            using (var command = new SQLiteCommand(Query.SelectVersionQuery, transaction.Connection))
            {
                command.SetTransaction(transaction).AddParam("Id", providerId);
                return (int?)command.ExecuteScalar();
            }
        }

        private static void AddEventSource(EventSourceInformation eventSource, SQLiteTransaction transaction)
        {
            using (var cmd = new SQLiteCommand(Query.InsertNewProviderQuery, transaction.Connection))
            {
                //TODO
                var eventSourceType = typeof (object);
                cmd.SetTransaction(transaction)
                    .AddParam("Id", eventSource.Id)
                    .AddParam("Type", eventSourceType.ToString())
                    .AddParam("Version", eventSource.CurrentVersion);
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveEvents(IEnumerable<UncommittedEvent> events, SQLiteTransaction transaction)
        {            
            foreach (var e in events)
            {
                SaveEvent(e, transaction);
            }
        }

        private void SaveEvent(UncommittedEvent evnt, SQLiteTransaction transaction)
        {
            if (evnt == null || transaction == null) throw new ArgumentNullException();
            using (var dataStream = new MemoryStream())
            {
                var bag = _converter.Convert(evnt.Payload);

                var formatter = new BinaryFormatter();
                formatter.Serialize(dataStream, bag);
                var data = dataStream.ToArray();

                using (var cmd = new SQLiteCommand(Query.InsertNewEventQuery, transaction.Connection))
                {
                    cmd.SetTransaction(transaction)
                        .AddParam("SourceId", evnt.EventSourceId)
                        .AddParam("EventId", evnt.EventIdentifier)
                        .AddParam("Name", evnt.Payload.GetType().FullName)
                        .AddParam("Sequence", evnt.EventSequence)                        
                        .AddParam("Timestamp", evnt.EventTimeStamp.Ticks)
                        .AddParam("Data", data);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateEventSources(UncommittedEventStream events, SQLiteTransaction transaction)
        {
            var eventSources = events.Sources;
            foreach (var eventSource in eventSources)
            {
                UpdateEventSourceVersion(eventSource, transaction);
            }
        }

        private static void UpdateEventSourceVersion(EventSourceInformation eventSource, SQLiteTransaction transaction)
        {
            using (var cmd = new SQLiteCommand(Query.UpdateEventSourceVersionQuery, transaction.Connection))
            {
                cmd.SetTransaction(transaction).AddParam("Id", eventSource.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void EnsureDatabaseExists(string connectionString)
        {
            var match = Regex.Match(connectionString, @"Data Source=(?<path>.*);", RegexOptions.IgnoreCase);
            if (!match.Success) throw new ArgumentException("Invalid connection string.");
            var path = match.Groups["path"].Value;
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                using (var conn = new SQLiteConnection(connectionString))
                using (var cmd = new SQLiteCommand(Query.CreateTables, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
