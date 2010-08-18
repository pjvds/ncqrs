using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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

        public IEnumerable<SourcedEvent> GetAllEvents(Guid id)
        {
            return GetAllEventsSinceVersion(id, 0);
        }

        public IEnumerable<SourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            var res = new List<SourcedEvent>();

            _context.WithConnection(connection =>
            {
                using (var cmd = new SQLiteCommand(Query.SelectAllEventsQuery, connection))
                {
                    cmd.AddParam("EventSourceId", id).AddParam("EventSourceVersion", version);

                    using (var reader = cmd.ExecuteReader())
                    {
                        var formatter = new BinaryFormatter();
                        while (reader.Read())
                        {
                            var rawData = (Byte[])reader["Data"];

                            using (var dataStream = new MemoryStream(rawData))
                            {
                                var bag = (PropertyBag)formatter.Deserialize(dataStream);
                                var evnt = (SourcedEvent)_converter.Convert(bag);
                                res.Add(evnt);
                            }
                        }
                    }
                }
            });

            return res;
        }

        public void Save(IEventSource source)
        {
            var events = source.GetUncommittedEvents();

            _context.WithConnection(connection => 
            _context.WithTransaction(connection, transaction =>
            {
                var currentVersion = GetVersion(source.EventSourceId, transaction);

                if (currentVersion == null)
                    AddEventSource(source, transaction);
                else if (currentVersion.Value != source.InitialVersion)
                    throw new ConcurrencyException(source.EventSourceId, source.Version);

                SaveEvents(events, source.EventSourceId, transaction);
                UpdateEventSourceVersion(source, transaction);
            }));    
        }

        private static int? GetVersion(Guid providerId, SQLiteTransaction transaction)
        {
            using (var command = new SQLiteCommand(Query.SelectVersionQuery, transaction.Connection))
            {
                command.SetTransaction(transaction).AddParam("Id", providerId);
                return (int?)command.ExecuteScalar();
            }
        }

        private void AddEventSource(IEventSource eventSource, SQLiteTransaction transaction)
        {
            using (var cmd = new SQLiteCommand(Query.InsertNewProviderQuery, transaction.Connection))
            {
                cmd.SetTransaction(transaction)
                    .AddParam("Id", eventSource.EventSourceId)
                    .AddParam("Type", eventSource.GetType().ToString())
                    .AddParam("Version", eventSource.Version);
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveEvents(IEnumerable<SourcedEvent> evnts, Guid eventSourceId, SQLiteTransaction transaction)
        {
            if (transaction == null || evnts == null) throw new ArgumentNullException();
            foreach (var e in evnts) SaveEvent(e, eventSourceId, transaction);
        }

        private void SaveEvent(SourcedEvent evnt, Guid eventSourceId, SQLiteTransaction transaction)
        {
            if (evnt == null || transaction == null) throw new ArgumentNullException();
            using (var dataStream = new MemoryStream())
            {
                var bag = _converter.Convert(evnt);

                var formatter = new BinaryFormatter();
                formatter.Serialize(dataStream, bag);
                var data = dataStream.ToArray();

                using (var cmd = new SQLiteCommand(Query.InsertNewEventQuery, transaction.Connection))
                {
                    cmd.SetTransaction(transaction)
                        .AddParam("Id", eventSourceId)
                        .AddParam("Name", evnt.GetType().FullName)
                        .AddParam("Sequence", evnt.EventSequence)
                        .AddParam("Timestamp", evnt.EventTimeStamp.Ticks)
                        .AddParam("Data", data);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateEventSourceVersion(IEventSource eventSource, SQLiteTransaction transaction)
        {
            using (var cmd = new SQLiteCommand(Query.UpdateEventSourceVersionQuery, transaction.Connection))
            {
                cmd.SetTransaction(transaction).AddParam("Id", eventSource.EventSourceId);
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