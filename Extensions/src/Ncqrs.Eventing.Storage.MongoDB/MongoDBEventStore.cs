using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Driver;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.MongoDB
{
    public class MongoDBEventStore : IEventStore
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The default data uri that points to a local Mongo DB.
        /// </summary>
        protected const string DEFAULT_DATABASE_URI = "mongo://127.0.0.1:27017/EventStore";

        protected readonly IDatabase database;

        public MongoDBEventStore()
            : this(DEFAULT_DATABASE_URI)
        {
        }

        public MongoDBEventStore(string databaseUri)
        {
            database = Mongo.GetDatabase(databaseUri);
        }

        public IEnumerable<ISourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            IDBCollection eventSources = database.GetCollection("Events");

            var query = new DBQuery("_SourceId", id.ToString());
            IDocument source = eventSources.FindOne();

            if (source == null) return new SourcedEvent[] { };

            var eventsAsDbObjects = ((DBObjectArray)source["_Events"]).Values.Cast<IDBObject>();

            //no benefit yield now we have single doc - might confused people due to lazy style invocation - esp if exception thrown
            var events = new List<SourcedEvent>();

            foreach (var eventDbObject in eventsAsDbObjects)
            {
                var evnt = DeserializeToEventIDBObject(eventDbObject);

                // TODO: Optimize: do not first serializing event before event sequence check.
                if (evnt.EventSequence > version)
                    events.Add(evnt);
            }

            // TODO: Add order to the query for optimization.
            return events.OrderBy(evnt => evnt.EventSequence);
        }

        public IEnumerable<ISourcedEvent> GetAllEvents(Guid id)
        {
            return GetAllEventsSinceVersion(id, 0);
        }

        public virtual void Save(IEventSource source)
        {
            IEnumerable<ISourcedEvent> eventsToSave = source.GetUncommittedEvents();

            IDBCollection sources = database.GetCollection("Events");

            if (IsNewEventSource(source))
            {
                InsertNewEventSource(source, eventsToSave, sources);
                VerifyInsertSuccessful(source);
            }
            else
            {
                PushOptimisticUpdate(source, eventsToSave, sources);
                VerifyUpdateSuccessful(source);
            }
        }

        private void InsertNewEventSource(IEventSource source, IEnumerable<ISourcedEvent> eventsToSave, IDBCollection sources)
        {
            var arrayOfEventsAsIdbObjects = GetArrayOfEventsAsIDBObjects(source, eventsToSave);
            var doc = new Document
                          {
                              {"_SourceId", source.EventSourceId.ToString()},
                              {"_Events", arrayOfEventsAsIdbObjects},
                              {"_Version", arrayOfEventsAsIdbObjects.Length} 
                          };

            // TODO: Add thread safe check. But, the driver should support checks on insert.
            sources.Insert(doc);
            VerifyInsertSuccessful(source);
        }

        private void PushOptimisticUpdate(IEventSource source, IEnumerable<ISourcedEvent> eventsToSave, IDBCollection sources)
        {
            var arrayOfEventsAsIdbObjects = GetArrayOfEventsAsIDBObjects(source, eventsToSave);
            sources.Update(new DBQuery()
                               {
                                   {"_SourceId", source.EventSourceId.ToString()},
                                   {"_Version", source.InitialVersion}
                               }
                           , Do.AddEachToSet("Events", arrayOfEventsAsIdbObjects
                                 ).Set("_Version", source.Version));
        }

        protected void VerifyInsertSuccessful(IEventSource source)
        {
            var lastError = database.GetLastError();
            var errorMessage = lastError.ErrorMessage;
            bool isInserted = String.IsNullOrEmpty(errorMessage);

            if(!isInserted)
            {
                throw new MongoException(errorMessage);
            }
        }

        protected void VerifyUpdateSuccessful(IEventSource source)
        {
            var lastError = database.GetLastError();
            var lastErrorData = lastError.Object as IDictionary<string, object>;
            var isUpdated = (bool)(lastErrorData["updatedExisting"]);
            if (!isUpdated)
            {
                throw new ConcurrencyException(source.EventSourceId, source.Version);
            }
        }

        protected IDBObject[] GetArrayOfEventsAsIDBObjects(IEventSource source, IEnumerable<ISourcedEvent> eventsToSave)
        {
            return eventsToSave.Select(ue => ConvertEventToIDBObject(source, ue)).ToArray();
        }

        private bool IsNewEventSource(IEventSource source)
        {
            return source.InitialVersion == 0;
        }

        protected static IDBObject ConvertEventToIDBObject(IEventSource eventSource, IEvent @event)
        {
            // TODO: cache propretyinfo collections per type
            PropertyInfo[] properties = @event.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var dbObject = new DBObject();
            dbObject["_SourceId"] = eventSource.EventSourceId.ToString();
            dbObject["_TimeStamp"] = DateTime.UtcNow;
            dbObject["_AssemblyQualifiedEventTypeName"] = @event.GetType().AssemblyQualifiedName;

            foreach (PropertyInfo prop in properties)
            {
                // we have to workaround the absence of Guid serialization in MongoDB driver
                dbObject[prop.Name] = prop.PropertyType.Equals(typeof(Guid))
                                          ? prop.GetValue(@event, new object[] { }).ToString()
                                          : prop.GetValue(@event, new object[] { });
            }

            return dbObject;
        }

        protected static SourcedEvent DeserializeToEventIDBObject(IDBObject dbObject)
        {
            Type eventType = Type.GetType((string)dbObject["_AssemblyQualifiedEventTypeName"]);

            var sourceId = Guid.Parse(dbObject["_SourceId"].ToString());

            var deserializedEvent = Activator.CreateInstance(eventType) as SourcedEvent;

            foreach (string key in dbObject.Keys)
            {
                var propertyOnEvent = eventType.GetProperty(key, BindingFlags.Public | BindingFlags.Instance);

                // TODO: Add warning to the log file when the prop was not found or writable.
                if (propertyOnEvent == null || !propertyOnEvent.CanWrite) continue;

                var propertyTypesMatch = propertyOnEvent.PropertyType.Equals(dbObject[key].GetType());

                if (propertyTypesMatch)
                {
                    propertyOnEvent.SetValue(deserializedEvent, dbObject[key], new object[] { });
                }

                var propertyOnEventIsGuidAndDbObjectPropertyIsString
                    = !propertyTypesMatch &&
                      propertyOnEvent.PropertyType.Equals(typeof(Guid)) &&
                      dbObject[key].GetType().Equals(typeof(string));

                if (propertyOnEventIsGuidAndDbObjectPropertyIsString)
                {
                    var parsedGuid = new Guid(dbObject[key].ToString());
                    propertyOnEvent.SetValue(deserializedEvent, parsedGuid, new object[] { });
                }
            }

            return deserializedEvent;
        }
    }
}