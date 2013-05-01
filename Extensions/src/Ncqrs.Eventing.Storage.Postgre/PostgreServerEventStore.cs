using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Text;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Data;

namespace Ncqrs.Eventing.Storage.Postgre
{
    /// <summary>Stores events for a SQL database.</summary>
    public class PostgreServerEventStore : IEventStore, ISnapshotStore
    {
        private readonly static int FirstVersion = 0;
        private readonly String _connectionString;

        private readonly IEventFormatter<JObject> _formatter;
        private readonly IEventTranslator<string> _translator;
        private readonly IEventConverter _converter;

        public PostgreServerEventStore(String connectionString) : this(connectionString, null, null) { }

        public PostgreServerEventStore(String connectionString, IEventTypeResolver typeResolver, IEventConverter converter)
        {
            if (String.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");

            _connectionString = connectionString;
            _converter = converter ?? new NullEventConverter();
            _formatter = new JsonEventFormatter(typeResolver ?? new SimpleEventTypeResolver());
            _translator = new StringEventTranslator();
        }

        private CommittedEvent ReadEventFromDbReader(NpgsqlDataReader reader)
        {
            StoredEvent<string> rawEvent = ReadEvent(reader);

            var document = _translator.TranslateToCommon(rawEvent);
            _converter.Upgrade(document);

            var payload = _formatter.Deserialize(document.Data, document.EventName);

            // TODO: Legacy stuff... we do not have a dummy id with the current schema.
            var dummyCommitId = Guid.Empty;
            var evnt = new CommittedEvent(dummyCommitId, document.EventIdentifier, document.EventSourceId, document.EventSequence, document.EventTimeStamp, payload, document.EventVersion);

            // TODO: Legacy stuff... should move.
            if (evnt is ISourcedEvent) { ((ISourcedEvent)evnt).InitializeFrom(rawEvent); }

            return evnt;
        }

        /// <summary>Saves a snapshot of the specified event source.</summary>
        public void SaveSnapshot(Snapshot snapshot)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                // Open connection and begin a transaction so we can
                // commit or rollback all the changes that has been made.
                connection.Open();
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var dataStream = new MemoryStream())
                        {
                            var formatter = new BinaryFormatter();
                            formatter.Serialize(dataStream, snapshot.Payload);
                            byte[] data = dataStream.ToArray();

                            using (var command = new NpgsqlCommand(Queries.InsertSnapshot, transaction.Connection))
                            {
                                command.Transaction = transaction;
                                command.Parameters.AddWithValue("EventSourceId", snapshot.EventSourceId);
                                command.Parameters.AddWithValue("Version", snapshot.Version);
                                command.Parameters.AddWithValue("Type", snapshot.GetType().AssemblyQualifiedName);
                                command.Parameters.AddWithValue("Data", data);
                                command.ExecuteNonQuery();
                            }
                        }

                        // Everything is handled, commit transaction.
                        transaction.Commit();
                    }
                    catch
                    {
                        // Something went wrong, rollback transaction.
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>Gets a snapshot of a particular event source, if one exists. Otherwise, returns <c>null</c>.</summary>
        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                // Open connection and begin a transaction so we can
                // commit or rollback all the changes that has been made.
                connection.Open();

                using (var command = new NpgsqlCommand(Queries.SelectLatestSnapshot, connection))
                {
                    command.Parameters.AddWithValue("@EventSourceId", eventSourceId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.FieldCount > 0)
                        {
                            var snapshotData = (byte[])reader["Data"];
                            using (var buffer = new MemoryStream(snapshotData))
                            {
                                var formatter = new BinaryFormatter();
                                var payload = formatter.Deserialize(buffer);
                                var theSnapshot = new Snapshot(eventSourceId, (long)reader["Version"], payload);

                                return theSnapshot.Version > maxVersion ? null : theSnapshot;
                            }
                        }

                        return null;
                    }
                }
            }
        }

        public IEnumerable<Guid> GetAllIdsForType(Type eventProviderType)
        {
            var ids = new List<Guid>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                using (var command = new NpgsqlCommand(Queries.SelectAllIdsForTypeQuery, connection))
                {
                    command.Parameters.AddWithValue("Type", eventProviderType.FullName);
                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(reader.GetGuid(0));
                        }
                    }
                }
            }

            return ids;
        }

        public void RemoveUnusedProviders()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                using (var command = new NpgsqlCommand(Queries.DeleteUnusedProviders, connection))
                {
                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void UpdateEventSourceVersion(Guid eventSourceId, long newVersion, NpgsqlTransaction transaction)
        {
            using (var command = new NpgsqlCommand(Queries.UpdateEventSourceVersionQuery, transaction.Connection))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("Id", eventSourceId);
                command.Parameters.AddWithValue("NewVersion", newVersion);
                command.ExecuteNonQuery();
            }
        }

        private StoredEvent<string> ReadEvent(NpgsqlDataReader reader)
        {
            var eventIdentifier = (Guid)reader["Id"];
            var eventTimeStamp = (DateTime)reader["TimeStamp"];
            var eventName = (string)reader["Name"];
            var eventVersion = Version.Parse((string)reader["Version"]);
            var eventSourceId = (Guid)reader["EventSourceId"];
            var eventSequence = (long)reader["Sequence"];
            var data = (String)reader["Data"];

            return new StoredEvent<string>(
                eventIdentifier,
                eventTimeStamp,
                eventName,
                eventVersion,
                eventSourceId,
                eventSequence,
                data);
        }

        /// <summary>Saves the events to the event store.</summary>
        /// <param name="evnts">The events to save.</param>
        /// <param name="eventSourceId">The event source id that owns the events.</param>
        /// <param name="transaction">The transaction.</param>
        private void SaveEvents(IEnumerable<UncommittedEvent> evnts, NpgsqlTransaction transaction)
        {
            Contract.Requires(evnts != null );
            Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<UncommittedEvent>>(), @event => @event != null));

            if (transaction == null)
                throw new ArgumentNullException("transaction");

            foreach (var sourcedEvent in evnts)
                SaveEvent(sourcedEvent, transaction);
        }

        /// <summary>Saves the event to the data store.</summary>
        /// <param name="evnt">The event to save.</param>
        /// <param name="transaction">The transaction.</param>
        private void SaveEvent(UncommittedEvent evnt, NpgsqlTransaction transaction)
        {
            string eventName;
            var document = _formatter.Serialize(evnt.Payload, out eventName);
            var storedEvent = new StoredEvent<JObject>(evnt.EventIdentifier, evnt.EventTimeStamp, eventName, evnt.EventVersion, evnt.EventSourceId, evnt.EventSequence, document);
            var raw = _translator.TranslateToRaw(storedEvent);

            using (var command = new NpgsqlCommand(Queries.InsertNewEventQuery, transaction.Connection))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("EventId", raw.EventIdentifier);
                command.Parameters.AddWithValue("TimeStamp", raw.EventTimeStamp);
                command.Parameters.AddWithValue("EventSourceId", raw.EventSourceId);
                command.Parameters.AddWithValue("Name", raw.EventName);
                command.Parameters.AddWithValue("Version", raw.EventVersion.ToString());
                command.Parameters.AddWithValue("Sequence", raw.EventSequence);
                command.Parameters.AddWithValue("Data", raw.Data);
                command.ExecuteNonQuery();
            }
        }

        private static void AddEventSource(Guid eventSourceId, Type eventSourceType, long initialVersion, NpgsqlTransaction transaction)
        {
            using (var command = new NpgsqlCommand(Queries.InsertNewProviderQuery, transaction.Connection))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("Id", eventSourceId);
                command.Parameters.AddWithValue("Type", eventSourceType.ToString());
                command.Parameters.AddWithValue("Version", initialVersion);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>Gets the version of the provider from the event store.</summary>
        /// <param name="providerId">The provider id.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>A <see cref="int?"/> that is <c>null</c> when no version was known ; otherwise,
        /// it contains the version number.</returns>
        private static int? GetVersion(Guid providerId, NpgsqlTransaction transaction)
        {
            using (var command = new NpgsqlCommand(Queries.SelectVersionQuery, transaction.Connection))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("id", providerId);
                return (int?)command.ExecuteScalar();
            }
        }

        /// <summary>Gets the table creation queries that can be used to create the tables that are needed
        /// for a database that is used as an event store.</summary>
        /// <remarks>This returns the content of the TableCreationScript.sql that is embedded as resource.</remarks>
        /// <returns>Queries that contain the <i>create table</i> statements.</returns>
        public static IEnumerable<String> GetTableCreationQueries()
        {
            const string resourcename = "Ncqrs.Eventing.Storage.Postgre.TableCreationScript.sql";

            var currentAsm = Assembly.GetExecutingAssembly();
            var resource = currentAsm.GetManifestResourceStream(resourcename);

            if (resource == null) 
                throw new ApplicationException("Could not find the resource " + resourcename + " in assembly " + currentAsm.FullName);

            var result = new List<string>();

            using (var reader = new StreamReader(resource))
            {
                string line = null;

                while ((line = reader.ReadLine()) != null)
                    result.Add(line);
            }

            return result;
        }

        /// <summary>Get some events after specified event.</summary>
        /// <param name="eventId">The id of last event not to be included in result set.</param>
        /// <param name="maxCount">Maximum numer of returned events</param>
        /// <returns>A collection events starting right after <paramref name="eventId"/>.</returns>
        public IEnumerable<CommittedEvent> GetEventsAfter(Guid? eventId, int maxCount)
        {
            var result = new List<CommittedEvent>();

            // Create connection and command.
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = eventId.HasValue
                    ? Queries.SelectEventsAfterQuery
                    : Queries.SelectEventsFromBeginningOfTime;

                using (var command = new NpgsqlCommand(string.Format(query, maxCount), connection))
                {
                    if (eventId.HasValue)
                    {
                        command.Parameters.AddWithValue("EventId", eventId);
                    }
                    connection.Open();

                    // Execute query and create reader.
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var evnt = ReadEventFromDbReader(reader);
                            result.Add(evnt);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>Reads from the stream from the <paramref name="minVersion"/> up until <paramref name="maxVersion"/>.</summary>
        /// <remarks>Returned event stream does not contain snapshots. This method is used when snapshots are stored in a separate store.</remarks>
        /// <param name="id">The id of the event source that owns the events.</param>
        /// <param name="minVersion">The minimum version number to be read.</param>
        /// <param name="maxVersion">The maximum version number to be read</param>
        /// <returns>All the events from the event source between specified version numbers.</returns>
        public CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion)
        {
            var events = new List<CommittedEvent>();

            // Create connection and command.
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                using (var command = new NpgsqlCommand(Queries.SelectAllEventsQuery, connection))
                {
                    // Add EventSourceId parameter and open connection.
                    command.Parameters.AddWithValue("EventSourceId", id);
                    command.Parameters.AddWithValue("EventSourceMinVersion", minVersion);
                    command.Parameters.AddWithValue("EventSourceMaxVersion", maxVersion);
                    connection.Open();

                    // Execute query and create reader.
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var evnt = ReadEventFromDbReader(reader);
                            events.Add(evnt);
                        }
                    }
                }
            }

            return new CommittedEventStream(id, events);
        }

        public void Store(UncommittedEventStream eventStream)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                try
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            if (eventStream.HasSingleSource)
                            {
                                var firstEvent = eventStream.First();
                                var newVersion = firstEvent.InitialVersionOfEventSource + eventStream.Count();
                                StoreEventsFromSource(eventStream.SourceId, newVersion, eventStream, transaction);
                            }
                            else
                            {
                                StoreMultipleSources(eventStream, transaction);
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void StoreEventsFromSource(Guid eventSourceId, long eventSourceVersion, IEnumerable<UncommittedEvent> events, NpgsqlTransaction transaction)
        {
            // Get the current version of the event provider.
            long? currentVersion = GetVersion(eventSourceId, transaction);
            long initialVersion = events.First().InitialVersionOfEventSource;

            // Create new event provider when it is not found.)
            if (currentVersion == null)
                AddEventSource(eventSourceId, typeof(object), eventSourceVersion, transaction);
            else if (currentVersion.Value != initialVersion)
                throw new ConcurrencyException(eventSourceId, eventSourceVersion);

            // Save all events to the store.
            SaveEvents(events, transaction);

            // Update the version of the provider.
            UpdateEventSourceVersion(eventSourceId, eventSourceVersion, transaction);
        }

        private void StoreMultipleSources(IEnumerable<UncommittedEvent> eventStreamContainingMultipleSources, NpgsqlTransaction transaction)
        {
            var sources = from evnt in eventStreamContainingMultipleSources
                          group evnt by evnt.EventSourceId into eventSourceGroup
                          select eventSourceGroup;

            foreach (var sourceStream in sources)
            {
                var firstEvent = sourceStream.First();
                var newVersion = firstEvent.InitialVersionOfEventSource + sourceStream.Count();
                StoreEventsFromSource(firstEvent.EventSourceId, newVersion, sourceStream, transaction);
            }
        }
    }
}