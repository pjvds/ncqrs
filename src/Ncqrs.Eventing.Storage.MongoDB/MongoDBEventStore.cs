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
        private readonly Mongo _mongo;
        private readonly string _databaseName;
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
        }

        /// <summary>
        /// Get all events provided by an specified event provider.
        /// </summary>
        /// <param name="id">The id of the event source that owns the events.</param>
        /// <returns>All the events from the event source.</returns>
        public IEnumerable<HistoricalEvent> GetAllEventsForEventSource(Guid id)
        {
            _mongo.Connect();

            try
            {
                var json = new MongoJson();
                var db = _mongo.GetDatabase(_databaseName);
                var collection = db.GetCollection(_collectionName);

                var specDocument = new Document();
                specDocument["EventSourceId"] = id;

                var foundDocuments = collection.Find(specDocument).Documents;

                foreach (var doc in foundDocuments)
                {
                    Type eventType = Type.GetType((string)doc["AssemblyQualifiedEventTypeName"]);
                    var deserializationResult = json.ObjectFrom(doc, eventType);
                    IEvent evnt = (IEvent)deserializationResult;

                    yield return new HistoricalEvent((DateTime)doc["TimeStamp"], evnt);
                }
            }
            finally
            {
                _mongo.Disconnect();
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
            _mongo.Connect();

            try
            {
                // TODO: Implement the ConcurrencyException check.
                var events = source.GetUncommitedEvents();

                var documents = GetAllDocumentsFromEventSource(source);

                var db = _mongo.GetDatabase(_databaseName);
                var collection = db.GetCollection(_collectionName);

                collection.Insert(documents, true);

                return events;
            }
            finally
            {
                _mongo.Disconnect();
            }
        }

        private IEnumerable<Document> GetAllDocumentsFromEventSource(EventSource eventSource)
        {
            foreach (var evnt in eventSource.GetUncommitedEvents())
            {
                var document = new Document();
                document["EventSourceId"] = eventSource.Id;
                document["TimeStamp"] = DateTime.UtcNow;
                document["AssemblyQualifiedEventTypeName"] = evnt.GetType().AssemblyQualifiedName;

                MongoJson json = new MongoJson();
                yield return json.PopulateDocumentFrom(document, evnt);
            }
        }
    }
}