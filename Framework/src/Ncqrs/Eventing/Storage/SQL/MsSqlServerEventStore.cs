using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

using Newtonsoft.Json.Linq;

using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Storage.Serialization;

namespace Ncqrs.Eventing.Storage.SQL
{
    /// <summary>Stores events for a SQL database.</summary>
    public class MsSqlServerEventStore : IEventStore, ISnapshotStore
    {
        private readonly IEventFormatter<JObject> _formatter;
        private readonly IEventTranslator<string> _translator;
        private readonly IEventConverter _converter;
        private readonly string _connectionString;
        int _initialized;

        /// <summary>Initializes a new instance of the <see cref="MsSqlServerEventStore"/> class.</summary>
        /// <param name="connectionString">The database connection string to the database.</param>
        public MsSqlServerEventStore(string connectionString) : this(connectionString, null, null)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                if (eventArgs.Name.Contains("DynamicSnapshot"))
                    return Assembly.LoadFrom("DynamicSnapshot.dll");
                return null;
            };
        }

        /// <summary>Initializes a new instance of the <see cref="MsSqlServerEventStore"/> class.</summary>
        /// <param name="connectionString">The database connection string to the database.</param>
        /// <param name="typeResolver">Indicates the <see cref="IEventTypeResolver"/> to use.</param>
        /// <param name="converter">Indicates the <see cref="IEventConverter"/> to use.</param>
        public MsSqlServerEventStore(string connectionString, IEventTypeResolver typeResolver, IEventConverter converter)
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            _connectionString = connectionString;
            _converter = converter ?? new NullEventConverter();
            _formatter = new JsonEventFormatter(typeResolver ?? new SimpleEventTypeResolver());
            _translator = new StringEventTranslator();

            InitializeEventStore();
        }

        private void InitializeEventStore()
        {
            if (Interlocked.Increment(ref _initialized) > 1)
                return;

            using (var connection = new SqlConnection(_connectionString))
            {
                var tableCreationScript = GetEventStoreTableCreationScript();
                var constraintCreationScript = GetEventStoreConstraintCreationScript();

                connection.Open();

                using (var command = new SqlCommand(tableCreationScript, connection))
                    command.ExecuteNonQuery();

                using (var command = new SqlCommand(constraintCreationScript, connection))
                    command.ExecuteNonQuery();
            }
        }

        /// <summary>Gets the table creation script that can be used to create the tables and indexes 
        /// that are needed for a database that is used as an event store.</summary>
        /// <remarks>This returns the content of the TableCreationScript.sql that is embedded as resource.</remarks>
        /// <returns>Queries that contain the <i>create table</i> and <i>create index</i> statements.</returns>
        public static IEnumerable<String> GetTableCreationQueries()
        {
            const string resourcename = "Ncqrs.Eventing.Storage.SQL.TableCreationScript.sql";

            var currentAsm = Assembly.GetExecutingAssembly();
            var resource = currentAsm.GetManifestResourceStream(resourcename);

            if (resource == null)
                throw new ApplicationException("Could not find the resource " + resourcename + " in assembly " + currentAsm.FullName);

            var result = new List<string>();

            using (var reader = new StreamReader(resource))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                    result.Add(line);
            }

            return result;
        }

        private static string GetEventStoreTableCreationScript()
        {
            const string resourcename = "Ncqrs.Eventing.Storage.SQL.TableCreationScript.sql";

            var currentAsm = Assembly.GetExecutingAssembly();
            var resource = currentAsm.GetManifestResourceStream(resourcename);

            if (resource == null)
                throw new ApplicationException("Could not find the resource " + resourcename + " in assembly " + currentAsm.FullName);

            using (var reader = new StreamReader(resource))
            {
                return reader.ReadToEnd();
            }
        }

        private static string GetEventStoreConstraintCreationScript()
        {
            const string resourcename = "Ncqrs.Eventing.Storage.SQL.ConstraintCreationScript.sql";

            var currentAsm = Assembly.GetExecutingAssembly();
            var resource = currentAsm.GetManifestResourceStream(resourcename);

            if (resource == null)
                throw new ApplicationException("Could not find the resource " + resourcename + " in assembly " + currentAsm.FullName);

            using (var reader = new StreamReader(resource))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>Returns back a list of all of the event source Id's that are in the store for the specified <paramref name="eventProviderType"/>.</summary>
        /// <param name="eventProviderType">Indicates the <see cref="Type"/> of the <see cref="EventSource"/> that publishes the events.</param>
        /// <returns>An enumeration of <see cref="Guid"/>.</returns>
        public IEnumerable<Guid> GetAllIdsForType(Type eventProviderType)
        {
            var ids = new List<Guid>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(Queries.SelectAllIdsForTypeQuery, connection))
                {
                    command.Parameters.AddWithValue("Type", eventProviderType.FullName);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add((Guid)reader[0]);
                        }
                    }
                }
            }

            return ids;
        }

        /// <summary>Get some events after specified event.</summary>
        /// <param name="eventId">The id of last event not to be included in result set.</param>
        /// <param name="maxCount">Maximum number of returned events</param>
        /// <returns>A collection events starting right after <paramref name="eventId"/>.</returns>
        public IEnumerable<CommittedEvent> GetEventsAfter(Guid? eventId, int maxCount)
        {
            var result = new List<CommittedEvent>();

            // Create connection and command.
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = eventId.HasValue ? Queries.SelectEventsAfterQuery : Queries.SelectEventsFromBeginningOfTime;

                using (var command = new SqlCommand(string.Format(query, maxCount), connection))
                {
                    if (eventId.HasValue)
                        command.Parameters.AddWithValue("EventId", eventId);

                    connection.Open();

                    // Execute query and create reader.
                    using (SqlDataReader reader = command.ExecuteReader())
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

        /// <summary>Gets a snapshot of a particular event source, if one exists. Otherwise, returns <c>null</c>.</summary>
        /// <param name="eventSourceId">Indicates the event source to retrieve the snapshot for.</param>
        /// <param name="maxVersion">Indicates the maximum allowed version to be returned.</param>
        /// <returns>Returns the most recent <see cref="Snapshot"/> that exists in the store. If the store has a 
        /// snapshot that is more recent than the <paramref name="maxVersion"/>, then <c>null</c> will be returned.</returns>
        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Open connection and begin a transaction so we can
                // commit or rollback all the changes that has been made.
                // QUESTION: Was this supposed to be done inside of a transaction?
                connection.Open();

                using (var command = new SqlCommand(Queries.SelectLatestSnapshot, connection))
                {
                    command.Parameters.AddWithValue("@EventSourceId", eventSourceId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var snapshotData = (byte[])reader["Data"];

                            using (var buffer = new MemoryStream(snapshotData))
                            {
                                var formatter = new BinaryFormatter();
                                var payload = formatter.Deserialize(buffer);
                                var theSnapshot = new Snapshot(eventSourceId, (long)reader["Version"], payload);

                                // QUESTION: Does it make sense to have this check performed in the SQL Query that way
                                // an older snapshot could be returned if it does exist?
                                return theSnapshot.Version > maxVersion ? null : theSnapshot;
                            }
                        }

                        return null;
                    }
                }
            }
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
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(Queries.SelectAllEventsQuery, connection))
                {
                    // Add EventSourceId parameter and open connection.
                    command.Parameters.AddWithValue("EventSourceId", id);
                    command.Parameters.AddWithValue("EventSourceMinVersion", minVersion);
                    command.Parameters.AddWithValue("EventSourceMaxVersion", maxVersion);
                    connection.Open();

                    // Execute query and create reader.
                    using (SqlDataReader reader = command.ExecuteReader())
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

        /// <summary>Removes any <see cref="EventSource"/> from the store that has not published any events.</summary>
        public void RemoveUnusedProviders()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(Queries.DeleteUnusedProviders, connection))
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

        /// <summary>Persists a <see cref="Snapshot"/> of an <see cref="EventSource"/>.</summary>
        /// <param name="snapshot">The <see cref="Snapshot"/> that is being saved.</param>
        public void SaveSnapshot(Snapshot snapshot)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Open connection and begin a transaction so we can
                // commit or rollback all the changes that has been made.
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var dataStream = new MemoryStream())
                        {
                            var formatter = new BinaryFormatter();
                            formatter.Serialize(dataStream, snapshot.Payload);
                            byte[] data = dataStream.ToArray();

                            using (var command = new SqlCommand(Queries.InsertSnapshot, transaction.Connection))
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

        /// <summary>Persists the <paramref name="eventStream"/> in the store as a single and atomic commit.</summary>
        /// <exception cref="ConcurrencyException">Occurs when there is already a newer version of the event provider stored in the event store.</exception>
        /// <param name="eventStream">The <see cref="UncommittedEventStream"/> to commit.</param>
        public void Store(UncommittedEventStream eventStream)
        {
            if (!eventStream.Any())
                return;

            using (var connection = new SqlConnection(_connectionString))
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

        /// <summary>Adds a new <see cref="EventSource"/> to the store.</summary>
        /// <param name="eventSourceId">Indicates the <see cref="EventSource.EventSourceId"/>.</param>
        /// <param name="eventSourceType">Indicates the concrete type that inherits from <see cref="EventSource"/>.</param>
        /// <param name="initialVersion">Indicates the initial version of the <see cref="EventSource"/>.</param>
        /// <param name="transaction">The <see cref="SqlTransaction"/> to enlist in while adding the <see cref="EventSource"/>.</param>
        private static void AddEventSource(Guid eventSourceId, Type eventSourceType, long initialVersion, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(Queries.InsertNewProviderQuery, transaction.Connection))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("Id", eventSourceId);
                command.Parameters.AddWithValue("Type", eventSourceType.ToString());
                command.Parameters.AddWithValue("Version", initialVersion);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>Gets the version of the <see cref="EventSource"/> from the event store.</summary>
        /// <param name="eventSourceId">The <see cref="EventSource.EventSourceId"/> to check for.</param>
        /// <param name="transaction">The <see cref="SqlTransaction"/> to enlist in.</param>
        /// <returns>A <see cref="Nullable{T}"/> of <see cref="int"/> that is <c>null</c> when no version was known ; otherwise,
        /// it contains the version number.</returns>
        private static int? GetVersion(Guid eventSourceId, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(Queries.SelectVersionQuery, transaction.Connection))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("id", eventSourceId);
                return (int?)command.ExecuteScalar();
            }
        }

        /// <summary>Translates a <see cref="IDataRecord"/> to a <see cref="StoredEvent"/></summary>
        /// <param name="reader">The current record to be translated.</param>
        /// <returns>A <see cref="StoredEvent"/>.</returns>
        private static StoredEvent<string> ReadEvent(IDataRecord reader)
        {
            var eventIdentifier = (Guid)reader["Id"];
            var eventTimeStamp = (DateTime)reader["TimeStamp"];
            var eventName = (string)reader["Name"];
            var eventVersion = Version.Parse((string)reader["Version"]);
            var eventSourceId = (Guid)reader["EventSourceId"];
            var eventSequence = (long)reader["Sequence"];
            var data = (string)reader["Data"];

            return new StoredEvent<string>(
                eventIdentifier,
                eventTimeStamp,
                eventName,
                eventVersion,
                eventSourceId,
                eventSequence,
                data);
        }

        /// <summary>Updates the version of the <see cref="EventSource"/> in the store with the new version.</summary>
        /// <param name="eventSourceId">The <see cref="EventSource.EventSourceId"/> that is being updated.</param>
        /// <param name="newVersion">Indicates the new version to use.</param>
        /// <param name="initialVersion">The initial version (used to ensure concurrency).</param>
        /// <param name="transaction">The transaction to enlist in while performing the action.</param>
        /// <returns>True if successfully updated</returns>
        private static bool UpdateEventSourceVersion(Guid eventSourceId, long newVersion, long initialVersion, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(Queries.UpdateEventSourceVersionQuery, transaction.Connection))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("Id", eventSourceId);
                command.Parameters.AddWithValue("NewVersion", newVersion);
                command.Parameters.AddWithValue("InitialVersion", initialVersion);
                var rowsUpdated = command.ExecuteNonQuery();
                return rowsUpdated != 0;
            }
        }

        /// <summary>Reads the current row in the <paramref name="reader"/> and translates it to a <see cref="CommittedEvent"/>.</summary>
        /// <param name="reader">The data record to read the current record from.</param>
        /// <returns>A fully populated <see cref="CommittedEvent"/> that contains the data from within the <paramref name="reader"/>.</returns>
        private CommittedEvent ReadEventFromDbReader(IDataRecord reader)
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

        /// <summary>Saves the events to the event store.</summary>
        /// <param name="uncommittedEvents">The events to save.</param>
        /// <param name="transaction">The transaction.</param>
        private void SaveEvents(IEnumerable<UncommittedEvent> uncommittedEvents, SqlTransaction transaction)
        {
            Contract.Requires<ArgumentNullException>(uncommittedEvents != null, "The argument uncommittedEvents could not be null.");
            Contract.Requires<ArgumentNullException>(transaction != null, "The argument transaction could not be null.");

            foreach (var sourcedEvent in uncommittedEvents)
            {
                SaveEvent(sourcedEvent, transaction);
            }
        }

        /// <summary>Saves the event to the data store.</summary>
        /// <param name="uncommittedEvent">The event to save.</param>
        /// <param name="transaction">The transaction.</param>
        private void SaveEvent(UncommittedEvent uncommittedEvent, SqlTransaction transaction)
        {
            Contract.Requires<ArgumentNullException>(uncommittedEvent != null, "The argument uncommittedEvent could not be null.");
            Contract.Requires<ArgumentNullException>(transaction != null, "The argument transaction could not be null.");

            string eventName;
            var document = _formatter.Serialize(uncommittedEvent.Payload, out eventName);
            var storedEvent = new StoredEvent<JObject>(uncommittedEvent.EventIdentifier, uncommittedEvent.EventTimeStamp,
                                                      eventName, uncommittedEvent.EventVersion, uncommittedEvent.EventSourceId,
                                                      uncommittedEvent.EventSequence, document);
            var raw = _translator.TranslateToRaw(storedEvent);

            using (var command = new SqlCommand(Queries.InsertNewEventQuery, transaction.Connection))
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

        /// <summary>Persists the enumeration of <see cref="UncommittedEvent"/> for the specified event source.</summary>
        /// <param name="eventSourceId">The id of the <see cref="EventSource"/> that the events are being persisted for.</param>
        /// <param name="eventSourceVersion">Indicates the version of the <see cref="EventSource"/> at the time that the events occurred.</param>
        /// <param name="events">The <see cref="UncommittedEvent"/>s that should be persisted.</param>
        /// <param name="transaction">Indicates <see cref="SqlTransaction"/> to enlist in while performing the insert.</param>
        /// <exception cref="ConcurrencyException">Is thrown when the version specified does not match the version in the store.</exception>
        private void StoreEventsFromSource(Guid eventSourceId, long eventSourceVersion, IEnumerable<UncommittedEvent> events, SqlTransaction transaction)
        {
            // Get the current version of the event provider.
            long? currentVersion = GetVersion(eventSourceId, transaction);
            long initialVersion = events.First().InitialVersionOfEventSource;

            // Create new event provider when it is not found.)
            if (currentVersion == null)
            {
                AddEventSource(eventSourceId, typeof(object), eventSourceVersion, transaction);
            }
            else if (currentVersion.Value != initialVersion)
            {
                throw new ConcurrencyException(eventSourceId, eventSourceVersion);
            }
            else
            {
                UpdateTheVersionOfTheEventSource(eventSourceId, eventSourceVersion, transaction, initialVersion);
            }

            // Save all events to the store.
            SaveEvents(events, transaction);
        }

        static void UpdateTheVersionOfTheEventSource(Guid eventSourceId, long eventSourceVersion, SqlTransaction transaction, long initialVersion)
        {
            var updated = UpdateEventSourceVersion(eventSourceId, eventSourceVersion, initialVersion, transaction);

            if (!updated)
            {
                throw new ConcurrencyException(eventSourceId, eventSourceVersion);
            }
        }

        /// <summary>Iterates through each <see cref="EventSource"/> in <paramref name="eventStreamContainingMultipleSources"/> and stores the events.</summary>
        /// <param name="eventStreamContainingMultipleSources">The <see cref="UncommittedEvent"/> to store.</param>
        /// <param name="transaction">The <see cref="SqlTransaction"/> to enlist while storing the events.</param>
        private void StoreMultipleSources(IEnumerable<UncommittedEvent> eventStreamContainingMultipleSources, SqlTransaction transaction)
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