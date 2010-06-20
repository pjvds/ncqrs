namespace Ncqrs.Eventing.Storage.SQLite {
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text.RegularExpressions;

    public class SQLiteEventStore:IEventStore {
        private readonly string _connectionString;

        public SQLiteEventStore(string connectionString) {
            _connectionString=connectionString;
        }

        public IEnumerable<ISourcedEvent> GetAllEvents(Guid id) {
            return GetAllEventsSinceVersion(id, 0);
        }

        public IEnumerable<ISourcedEvent> GetAllEventsSinceVersion(Guid id, long version) {
            var res=new List<ISourcedEvent>();
            using (var conn=new SQLiteConnection(_connectionString))
            using (var cmd=new SQLiteCommand(Query.SelectAllEventsQuery, conn)) {
                cmd.AddParam("EventSourceId", id).AddParam("EventSourceVersion", version);
                conn.Open();
                using (var reader=cmd.ExecuteReader()) {
                    var formatter=new BinaryFormatter();
                    while (reader.Read()) {
                        var rawData=(Byte[])reader["Data"];

                        using (var dataStream=new MemoryStream(rawData)) {
                            var evnt=(ISourcedEvent)formatter.Deserialize(dataStream);
                            res.Add(evnt);
                        }
                    }
                }
            }

            return res;
        }

        public void Save(IEventSource source) {
            var events=source.GetUncommittedEvents();
            using (var conn=new SQLiteConnection(_connectionString)) {
                conn.Open();
                using (var transaction=conn.BeginTransaction())
                    try {
                        var currentVersion=GetVersion(source.Id, transaction);

                        if (currentVersion==null)
                            AddEventSource(source, transaction);
                        else if (currentVersion.Value!=source.InitialVersion)
                            throw new ConcurrencyException(source.Id, source.Version);

                        SaveEvents(events, source.Id, transaction);
                        UpdateEventSourceVersion(source, transaction);
                        transaction.Commit();
                    } catch {
                        transaction.Rollback();
                        throw;
                    }
            }
        }

        private static int? GetVersion(Guid providerId, SQLiteTransaction transaction) {
            using (var command=new SQLiteCommand(Query.SelectVersionQuery, transaction.Connection)) {
                command.SetTransaction(transaction).AddParam("Id", providerId);
                return (int?)command.ExecuteScalar();
            }
        }


        private static void AddEventSource(IEventSource eventSource, SQLiteTransaction transaction) {
            using (var cmd=new SQLiteCommand(Query.InsertNewProviderQuery, transaction.Connection)) {
                cmd.SetTransaction(transaction)
                    .AddParam("Id", eventSource.Id)
                    .AddParam("Type", eventSource.GetType().ToString())
                    .AddParam("Version", eventSource.Version);
                cmd.ExecuteNonQuery();
            }
        }

        private static void SaveEvents(IEnumerable<ISourcedEvent> evnts, Guid eventSourceId, SQLiteTransaction transaction) {
            if (transaction==null||evnts==null) throw new ArgumentNullException();
            foreach (var e in evnts) SaveEvent(e, eventSourceId, transaction);
        }

        private static void SaveEvent(ISourcedEvent evnt, Guid eventSourceId, SQLiteTransaction transaction) {
            if (evnt==null||transaction==null) throw new ArgumentNullException();
            using (var dataStream=new MemoryStream()) {
                var formatter=new BinaryFormatter();
                formatter.Serialize(dataStream, evnt);
                var data=dataStream.ToArray();

                using (var cmd=new SQLiteCommand(Query.InsertNewEventQuery, transaction.Connection)) {
                    cmd.SetTransaction(transaction)
                        .AddParam("Id", eventSourceId)
                        .AddParam("Name", evnt.GetType().FullName)
                        .AddParam("Sequence", evnt.EventSequence)
                        .AddParam("Data", data);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void UpdateEventSourceVersion(IEventSource eventSource, SQLiteTransaction transaction) {
            using (var cmd=new SQLiteCommand(Query.UpdateEventSourceVersionQuery, transaction.Connection)) {
                cmd.SetTransaction(transaction).AddParam("Id", eventSource.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void EnsureDatabaseExists(string connectionString) {
            var match=Regex.Match(connectionString, @"Data Source=(?<path>.*);", RegexOptions.IgnoreCase);
            if (!match.Success) throw new ArgumentException("Invalid connection string.");
            var path=match.Groups["path"].Value;
            if (!File.Exists(path)) {
                SQLiteConnection.CreateFile(path);
                using (var conn=new SQLiteConnection(connectionString))
                using (var cmd=new SQLiteCommand(Query.CreateTables, conn)) {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}