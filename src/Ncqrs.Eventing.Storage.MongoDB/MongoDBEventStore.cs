using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Diagnostics.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;

namespace Ncqrs.Eventing.Storage.MongoDB
{
    /// <summary>
    /// An event store that uses MongoDB as storage mechanism. MongoDB is an document based database. 
    /// See <see cref="http://mongodb.org"/> for more information about MongoDB.
    /// </summary>
    /// <remarks>
    /// Be aware of the fact that MongoDB doesn't support transactions at the moment!
    /// </remarks>
    public class MongoDBEventStore : IEventStore
    {
        /// <summary>
        /// The log to use to log messages from this instance.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The reference to Mongo.
        /// </summary>
        private readonly Mongo _mongo;

        /// <summary>
        /// The name of the database.
        /// </summary>
        private readonly string _databaseName;

        /// <summary>
        /// The name of the collection that contains all the events.
        /// </summary>
        private readonly string _collectionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBEventStore"/> class.
        /// </summary>
        /// <param name="mongo">The mongo to connect to.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>mongo</i> is <c>null</c>.</exception>
        public MongoDBEventStore(Mongo mongo)
            : this(mongo, "EventStore")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBEventStore"/> class.
        /// </summary>
        /// <param name="mongo">The mongo to connect to.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>mongo</i>, or <i>databaseName</i> is <c>null</c> or empty.</exception>
        public MongoDBEventStore(Mongo mongo, String databaseName)
            : this(mongo, databaseName, "Events")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBEventStore"/> class.
        /// </summary>
        /// <param name="mongo">The mongo to connect to.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <exception cref="ArgumentNullException">Occurs when <i>mongo</i>, <i>databaseName</i>, or <i>collectionName</i> is <c>null</c> or empty.</exception>
        public MongoDBEventStore(Mongo mongo, String databaseName, String collectionName)
        {
            Contract.Requires<ArgumentNullException>(mongo != null);
            Contract.Requires<ArgumentNullException>(!String.IsNullOrEmpty(databaseName));
            Contract.Requires<ArgumentNullException>(!String.IsNullOrEmpty(collectionName));

            _mongo = mongo;
            _databaseName = databaseName;
            _collectionName = collectionName;

            Log.DebugFormat("Initialized a new instance of MongoDBEventStore with the databasename set to {0} and collection set to {1}.", databaseName, collectionName);
        }

        /// <summary>
        /// Get all events provided by an specified event provider.
        /// </summary>
        /// <param name="id">The id of the event source that owns the events.</param>
        /// <returns>All the events from the event source.</returns>
        public IEnumerable<HistoricalEvent> GetAllEventsForEventSource(Guid id)
        {
            // Connect to Mongo.
            _mongo.Connect();
            Log.DebugFormat("Connected to Mogno at {0}.", _mongo.ConnectionString);

            try
            {
                // Get the collection.
                var db = _mongo.GetDatabase(_databaseName);
                var collection = db.GetCollection(_collectionName);

                Log.DebugFormat("Getting all events from {0} for event source with id {1}.", collection.FullName, id);

                // Create sample document.
                var exampleDoc = new Document();
                exampleDoc["EventSourceId"] = id;

                // Get documents.
                var cursor = collection.Find(exampleDoc);
                var foundDocuments = cursor.Documents;

                Log.DebugFormat("Found {0} events for event source with id {1}.", collection.FullName, foundDocuments.Count());

                foreach (var doc in foundDocuments)
                {
                    yield return DeserializeDocument(doc);
                }
            }
            finally
            {
                // Make sure we disconnect.
                _mongo.Disconnect();
                Log.Debug("Disconnected from Mogno.");
            }
        }

        /// <summary>
        /// Save all events from a specific event provider.
        /// </summary>
        /// <param name="source">The source that should be saved.</param>
        /// <returns></returns>
        /// <exception cref="ConcurrencyException">Occurs when there is already a newer version of the event provider stored in the event store.</exception>
        public IEnumerable<IEvent> Save(EventSource source)
        {
            // The events that are saved during this operation.
            // This will be returned at the end.
            IEnumerable<IEvent> savedEvents = new IEvent[0];

            // Get all events to save.
            var eventsToSave = source.GetUncommitedEvents();

            // Only save events when they are available.
            if (eventsToSave.Count() > 0)
            {
                // Connect to Mongo.
                _mongo.Connect();
                Log.DebugFormat("Connected to Mogno at {0}.", _mongo.ConnectionString);

                try
                {
                    // Get the right collection.
                    var db = _mongo.GetDatabase(_databaseName);
                    var collection = db.GetCollection(_collectionName);

                    // Make sure the source version matched with the version in the store.
                    var currentVersionInStore = GetVersion(collection, source);
                    if (currentVersionInStore != source.Version)
                    {
                        // Log error.
                        Log.ErrorFormat("Unable to save events for event source with id {0}. Since "+
                                        "the version in the store is {1} and the version of the event "+
                                        "source to save is {2}.", source.Id, currentVersionInStore, source.Version);

                        throw new ConcurrencyException(source.Version, currentVersionInStore);
                    }

                    // Get all events as documents.
                    var documents = GetAllDocumentsFromEventSource(source);

                    // Save the documents.
                    collection.Insert(documents, true);

                    // Set saved events, they will be returned on exit.
                    savedEvents = eventsToSave;
                    Log.DebugFormat("Saved all events for event source with id {0}.", source.Id);
                }
                finally
                {
                    // Make sure we disconnect.
                    _mongo.Disconnect();
                    Log.Debug("Disconnected from Mogno.");
                }
            }
            else
            {
                Log.DebugFormat("No events to save for event source with id {0}.", source.Id);
            }

            return savedEvents;
        }

        /// <summary>
        /// Gets the version from the event store for an event source.
        /// </summary>
        /// <param name="eventsCollection">The events collection.</param>
        /// <param name="source">The event source.</param>
        /// <returns>The version in the event store for the specified event source.</returns>
        private static long GetVersion(IMongoCollection eventsCollection, EventSource source)
        {
            var exampleDoc = new Document();
            exampleDoc["EventSourceId"] = source.Id;
            long version = eventsCollection.Count(exampleDoc);

            return version;
        }

        /// <summary>
        /// Gets all documents from event source.
        /// </summary>
        /// <param name="eventSource">The event source.</param>
        /// <returns>All the documents for the specified event source.</returns>
        private IEnumerable<Document> GetAllDocumentsFromEventSource(EventSource eventSource)
        {
            foreach (var evnt in eventSource.GetUncommitedEvents())
            {
                var document = new Document();
                document["EventSourceId"] = eventSource.Id;
                document["TimeStamp"] = DateTime.UtcNow;
                document["AssemblyQualifiedEventTypeName"] = evnt.GetType().AssemblyQualifiedName;

                yield return SerializeEventIntoDocument(document, evnt);
            }
        }

        /// <summary>
        /// Serializes the event into a spefified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="evnt">The event.</param>
        /// <returns>The document with the added data.</returns>
        private static Document SerializeEventIntoDocument(Document document, IEvent evnt)
        {
            var json = JsonConvert.SerializeObject(evnt, Formatting.None);
            var keyValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            foreach (var keyValue in keyValues)
            {
                var isEmptyKeyField = (keyValue.Key == "_id" && document["_id"] != null);

                if (isEmptyKeyField)
                    continue;

                var value = keyValue.Value ?? null;

                if (value != null)
                {
                    var arrayValue = (keyValue.Value as JArray);
                    if (arrayValue != null)
                        value = arrayValue.Select(j => (string)j).ToArray();
                }

                if (document.Contains(keyValue.Key))
                    document[keyValue.Key] = value;
                else
                {
                    if (value != null)
                        document.Add(keyValue.Key, value);
                }
            }

            return document;
        }

        /// <summary>
        /// Deserializes the document into a historical event.
        /// </summary>
        /// <param name="doc">The document to deserialize.</param>
        /// <returns>A new historical event that was deserialized from the document.</returns>
        private HistoricalEvent DeserializeDocument(Document doc)
        {
            Type eventType = Type.GetType((string)doc["AssemblyQualifiedEventTypeName"]);
            
            string json = doc.ToString();
            IEvent evnt = (IEvent)JsonConvert.DeserializeObject(json, eventType);

            return new HistoricalEvent((DateTime)doc["TimeStamp"], evnt);
        }
    }
}