using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Eventing.Storage.SQL
{
    /// <summary>
    /// Stores events for a SQL database.
    /// </summary>
    public class SimpleMicrosoftSqlServerEventStore : IEventStore, ISnapshotStore
    {
        #region Queries
        private const String DeleteUnusedProviders = "DELETE FROM [EventSources] WHERE (SELECT Count(EventSourceId) FROM [Events] WHERE [EventSourceId]=[EventSources].[Id]) = 0";

        private const String InsertNewEventQuery = "INSERT INTO [Events]([Id], [EventSourceId], [Name], [Data], [Sequence], [TimeStamp]) VALUES (@EventId, @EventSourceId, @Name, @Data, @Sequence, getDate())";

        private const String InsertNewProviderQuery = "INSERT INTO [EventSources](Id, Type, Version) VALUES (@Id, @Type, @Version)";

        private const String SelectAllEventsQuery = "SELECT [TimeStamp], [Data], [Sequence] FROM [Events] WHERE [EventSourceId] = @EventSourceId AND [Sequence] > @EventSourceVersion ORDER BY [Sequence]";

        private const String SelectAllIdsForTypeQuery = "SELECT [Id] FROM [EventSources] WHERE [Type] = @Type";

        private const String SelectVersionQuery = "SELECT [Version] FROM [EventSources] WHERE [Id] = @id";

        private const String UpdateEventSourceVersionQuery = "UPDATE [EventSources] SET [Version] = (SELECT Count(*) FROM [Events] WHERE [EventSourceId] = @Id) WHERE [Id] = @id";

        private const String InsertSnapshot = "DELETE FROM [Snapshots] WHERE [EventSourceId]=@EventSourceId; INSERT INTO [Snapshots]([EventSourceId], [Version], [SnapshotType], [SnapshotData]) VALUES (@EventSourceId, @Version, @SnapshotType, @SnapshotData)";

        private const String SelectLatestSnapshot = "SELECT TOP 1 * FROM [Snapshots] WHERE [EventSourceId]=@EventSourceId ORDER BY Version DESC";
        #endregion
        
        private readonly String _connectionString;

        public SimpleMicrosoftSqlServerEventStore(String connectionString)
        {
            if (String.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");

            _connectionString = connectionString;
        }

        /// <summary>
        /// Get all event for a specific event provider.
        /// </summary>
        /// <param name="id">The id of the event provider.</param>
        /// <returns>All events for the specified event provider.</returns>
        public IEnumerable<SourcedEvent> GetAllEvents(Guid id)
        {
            return GetAllEventsSinceVersion(id, 0);
        }

        /// <summary>
        /// Get all events provided by an specified event source.
        /// </summary>
        /// <param name="eventSourceId">The id of the event source that owns the events.</param>
        /// <returns>All the events from the event source.</returns>
        public IEnumerable<SourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            var result = new List<SourcedEvent>();

            // Create connection and command.
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(SelectAllEventsQuery, connection))
            {
                // Add EventSourceId parameter and open connection.
                command.Parameters.AddWithValue("EventSourceId", id);
                command.Parameters.AddWithValue("EventSourceVersion", version);
                connection.Open();

                // Execute query and create reader.
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Create formatter that can deserialize our events.
                    var formatter = new BinaryFormatter();

                    while (reader.Read())
                    {
                        // Get event details.
                        var rawData = (Byte[])reader["Data"];

                        using (var dataStream = new MemoryStream(rawData))
                        {
                            var evnt = (SourcedEvent)formatter.Deserialize(dataStream);
                            result.Add(evnt);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Saves all events from an event provider.
        /// </summary>
        /// <param name="provider">The eventsource.</param>
        public void Save(IEventSource eventSource)
        {
            // Get all events.
            IEnumerable<ISourcedEvent> events = eventSource.GetUncommittedEvents();

            // Create new connection.
            using (var connection = new SqlConnection(_connectionString))
            {
                // Open connection and begin a transaction so we can
                // commit or rollback all the changes that has been made.
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Get the current version of the event provider.
                        int? currentVersion = GetVersion(eventSource.Id, transaction);

                        // Create new event provider when it is not found.
                        if (currentVersion == null)
                        {
                            AddEventSource(eventSource, transaction);
                        }
                        else if (currentVersion.Value != eventSource.InitialVersion)
                        {
                            throw new ConcurrencyException(eventSource.Id, eventSource.Version);
                        }

                        // Save all events to the store.
                        SaveEvents(events, transaction);

                        // Update the version of the provider.
                        UpdateEventSourceVersion(eventSource, transaction);

                        // Everything is handled, commint transaction.
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

        /// <summary>
        /// Saves a snapshot of the specified event source.
        /// </summary>
        public void SaveShapshot(ISnapshot snapshot)
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
                            formatter.Serialize(dataStream, snapshot);
                            byte[] data = dataStream.ToArray();

                            using (var command = new SqlCommand(InsertSnapshot, transaction.Connection))
                            {
                                command.Transaction = transaction;
                                command.Parameters.AddWithValue("EventSourceId", snapshot.EventSourceId);
                                command.Parameters.AddWithValue("Version", snapshot.EventSourceVersion);
                                command.Parameters.AddWithValue("SnapshotType", snapshot.GetType().AssemblyQualifiedName);
                                command.Parameters.AddWithValue("SnapshotData", data);
                                command.ExecuteNonQuery();
                            }
                        }

                        // Everything is handled, commint transaction.
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

        /// <summary>
        /// Gets a snapshot of a particular event source, if one exists. Otherwise, returns <c>null</c>.
        /// </summary>
        public ISnapshot GetSnapshot(Guid eventSourceId)
        {
            ISnapshot theSnapshot = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                // Open connection and begin a transaction so we can
                // commit or rollback all the changes that has been made.
                connection.Open();

                using (var command = new SqlCommand(SelectLatestSnapshot, connection))
                {
                    command.Parameters.AddWithValue("@EventSourceId", eventSourceId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var version = (long) reader["Version"];
                            var snapshotData = (byte[]) reader["SnapshotData"];
                            using (var buffer = new MemoryStream(snapshotData))
                            {
                                var formatter = new BinaryFormatter();
                                theSnapshot = (ISnapshot) formatter.Deserialize(buffer);
                            }
                        }
                    }
                }
            }

            return theSnapshot;
        }

        public IEnumerable<Guid> GetAllIdsForType(Type eventProviderType)
        {
            // Create connection and command.
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(SelectAllIdsForTypeQuery, connection))
            {
                // Add EventSourceId parameter and open connection.
                command.Parameters.AddWithValue("Type", eventProviderType.FullName);
                connection.Open();

                // Execute query and create reader.
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return (Guid)reader[0];
                    }
                }
            }
        }

        public void RemoveUnusedProviders()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(DeleteUnusedProviders, connection))
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

        private static void UpdateEventSourceVersion(IEventSource eventSource, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(UpdateEventSourceVersionQuery, transaction.Connection))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("Id", eventSource.Id);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Saves the events to the event store.
        /// </summary>
        /// <param name="evnts">The events to save.</param>
        /// <param name="eventSourceId">The event source id that owns the events.</param>
        /// <param name="transaction">The transaction.</param>
        private static void SaveEvents(IEnumerable<ISourcedEvent> evnts, SqlTransaction transaction)
        {
            Contract.Requires<ArgumentNullException>(evnts != null, "The argument evnts could not be null.");
            Contract.Requires<ArgumentNullException>(transaction != null, "The argument transaction could not be null.");

            foreach (var sourcedEvent in evnts)
            {
                SaveEvent(sourcedEvent, transaction);
            }
        }

        /// <summary>
        /// Saves the event to the data store.
        /// </summary>
        /// <param name="evnt">The event to save.</param>
        /// <param name="eventSourceId">The id of the event source that owns the event.</param>
        /// <param name="transaction">The transaction.</param>
        private static void SaveEvent(ISourcedEvent evnt, SqlTransaction transaction)
        {
            Contract.Requires<ArgumentNullException>(evnt != null, "The argument evnt could not be null.");
            Contract.Requires<ArgumentNullException>(transaction != null, "The argument transaction could not be null.");

            using (var dataStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(dataStream, evnt);
                byte[] data = dataStream.ToArray();

                using (var command = new SqlCommand(InsertNewEventQuery, transaction.Connection))
                {
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("EventId", evnt.EventIdentifier);
                    command.Parameters.AddWithValue("EventSourceId", evnt.EventSourceId);
                    command.Parameters.AddWithValue("Name", evnt.GetType().FullName);
                    command.Parameters.AddWithValue("Sequence", evnt.EventSequence);
                    command.Parameters.AddWithValue("Data", data);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Adds the event source to the event store.
        /// </summary>
        /// <param name="eventSource">The event source to add.</param>
        /// <param name="transaction">The transaction.</param>
        private static void AddEventSource(IEventSource eventSource, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(InsertNewProviderQuery, transaction.Connection))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("Id", eventSource.Id);
                command.Parameters.AddWithValue("Type", eventSource.GetType().ToString());
                command.Parameters.AddWithValue("Version", eventSource.Version);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gets the version of the provider from the event store.
        /// </summary>
        /// <param name="providerId">The provider id.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>A <see cref="int?"/> that is <c>null</c> when no version was known ; otherwise,
        /// it contains the version number.</returns>
        private static int? GetVersion(Guid providerId, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(SelectVersionQuery, transaction.Connection))
            {
                command.Transaction = transaction;
                command.Parameters.AddWithValue("id", providerId);
                return (int?)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Gets the table creation queries that can be used to create the tables that are needed
        /// for a database that is used as an event store.
        /// </summary>
        /// <remarks>This returns the content of the TableCreationScript.sql that is embedded as resource.</remarks>
        /// <returns>Queries that contain the <i>create table</i> statements.</returns>
        public static IEnumerable<String> GetTableCreationQueries()
        {
            var currentAsm = Assembly.GetExecutingAssembly();

            const string resourcename = "Ncqrs.Eventing.Storage.SQL.TableCreationScript.sql";
            var resource = currentAsm.GetManifestResourceStream(resourcename);

            if (resource == null) throw new ApplicationException("Could not find the resource " + resourcename + " in assembly " + currentAsm.FullName);

            var result = new List<string>();
            
            using(var reader = new StreamReader(resource))
            {
                string line = null;
                while((line = reader.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }

            return result;
        }
    }
}