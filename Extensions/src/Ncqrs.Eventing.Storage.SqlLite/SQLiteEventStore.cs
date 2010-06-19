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
        private readonly string _connectionString;
        private IPropertyBagConverter _converter;

        public SQLiteEventStore(string connectionString) : this(connectionString, new PropertyBagConverter())
        {
        }

        public SQLiteEventStore(string connectionString, IPropertyBagConverter converter)
        {
            _connectionString = connectionString;
            _converter = converter;
        }

        public IEnumerable<SourcedEvent> GetAllEvents(Guid id)
        {
            return GetAllEventsSinceVersion(id, 0);
        }

        public IEnumerable<SourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            var res = new List<SourcedEvent>();
            using (var conn = new SQLiteConnection(_connectionString))
            using (var cmd = new SQLiteCommand(Query.SelectAllEventsQuery, conn))
            {
                cmd.AddParam("EventSourceId", id).AddParam("EventSourceVersion", version);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    var formatter = new BinaryFormatter();
                    while (reader.Read())
                    {
                        var rawData = (Byte[])reader["Data"];

                        using (var dataStream = new MemoryStream(rawData))
                        {
                            var bag = (PropertyBag) formatter.Deserialize(dataStream);
                            var evnt = (SourcedEvent)_converter.Convert(bag);
                            res.Add(evnt);
                        }
                    }
                }
            }

            return res;
        }

        public void Save(IEventSource source)
        {
            var events = source.GetUncommittedEvents();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                    try
                    {
                        var currentVersion = GetVersion(source.EventSourceId, transaction);

                        if (currentVersion == null)
                            AddEventSource(source, transaction);
                        else if (currentVersion.Value != source.InitialVersion)
                            throw new ConcurrencyException(source.EventSourceId, source.Version);

                        SaveEvents(events, source.EventSourceId, transaction);
                        UpdateEventSourceVersion(source, transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
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