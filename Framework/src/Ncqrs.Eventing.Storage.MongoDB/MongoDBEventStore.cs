using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using MongoDB.Driver;

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

        public virtual IEnumerable<IEvent> GetAllEventsForEventSource(Guid id)
        {
            IDBCollection aggregates = database.GetCollection("Events");

            IDocument aggregate = aggregates.FindOne(new DBQuery("SourceId", id.ToString()));

            if (aggregate == null) return new IEvent[] { };

            var eventsAsDbObjects = ((DBObjectArray)aggregate["Events"]).Values.Cast<IDBObject>();

            //no benefit yield now we have single doc - might confused people due to lazy style invocation - esp if exception thrown
            var events = new List<IEvent>();

            foreach (var eventDbObject in eventsAsDbObjects)
            {
                events.Add(DeserializeToEventIDBObject(eventDbObject));
            }

            return events;
        }

        public virtual IEnumerable<IEvent> Save(IEventSource source)
        {
            IEnumerable<ISourcedEvent> eventsToSave = source.GetUncommittedEvents();

            if (eventsToSave.Count() == 0) return new IEvent[] { };

            IDBCollection aggregates = database.GetCollection("Events");

            if (IsNewAggregate(source))
            {
                InsertNewAggregate(source, eventsToSave, aggregates);
            }
            else
            {
                PushOptimisticUpdate(source, eventsToSave, aggregates);
                VerifyUpdateSuccessful(source);
            }

            return eventsToSave;
        }

        private void InsertNewAggregate(IEventSource source, IEnumerable<ISourcedEvent> eventsToSave, IDBCollection aggregates)
        {
            var arrayOfEventsAsIdbObjects = GetArrayOfEventsAsIDBObjects(source, eventsToSave);
            var doc = new Document
                          {
                              {"SourceId", source.Id.ToString()},
                              {"Events", arrayOfEventsAsIdbObjects},
                              {"Version", arrayOfEventsAsIdbObjects.Length} 
                          };

            aggregates.Insert(doc);
        }

        private void PushOptimisticUpdate(IEventSource source, IEnumerable<ISourcedEvent> eventsToSave, IDBCollection aggregates)
        {
            var arrayOfEventsAsIdbObjects = GetArrayOfEventsAsIDBObjects(source, eventsToSave);
            aggregates.Update(new DBQuery()
                                  {
                                      {"SourceId", source.Id.ToString()},
                                      {"Version", source.Version}
                                  }
                              , Do.AddEachToSet("Events", arrayOfEventsAsIdbObjects
                                    ).Inc("Version", arrayOfEventsAsIdbObjects.Length));
        }

        protected void VerifyUpdateSuccessful(IEventSource source)
        {
            var lastError = database.GetLastError();
            var lastErrorData = lastError.Object as IDictionary<string, object>;
            var isUpdated = (bool)(lastErrorData["updatedExisting"]);
            if (!isUpdated)
            {
                //TODO: Optimistic concurrency check does not pull out the version in store number by design. Change exception.
                throw new ConcurrencyException(source.Id, source.Version, -1);
            }
        }

        protected IDBObject[] GetArrayOfEventsAsIDBObjects(IEventSource source, IEnumerable<ISourcedEvent> eventsToSave)
        {
            return eventsToSave.Select(ue => ConvertEventToIDBObject(source, ue)).ToArray();
        }

        private bool IsNewAggregate(IEventSource source)
        {
            return source.Version == 0;
        }

        protected static IDBObject ConvertEventToIDBObject(IEventSource eventSource, IEvent @event)
        {
            // TODO: cache propretyinfo collections per type
            PropertyInfo[] properties = @event.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var dbObject = new DBObject();
            dbObject["SourceId"] = eventSource.Id.ToString();
            dbObject["TimeStamp"] = DateTime.UtcNow;
            dbObject["AssemblyQualifiedEventTypeName"] = @event.GetType().AssemblyQualifiedName;

            foreach (PropertyInfo prop in properties)
            {
                // we have to workaround the absence of Guid serialization in MongoDB driver
                dbObject[prop.Name] = prop.PropertyType.Equals(typeof(Guid))
                                          ? prop.GetValue(@event, new object[] { }).ToString()
                                          : prop.GetValue(@event, new object[] { });
            }

            return dbObject;
        }

        protected static IEvent DeserializeToEventIDBObject(IDBObject dbObject)
        {
            Type eventType = Type.GetType((string)dbObject["AssemblyQualifiedEventTypeName"]);

            var aggId = Guid.Parse(dbObject["SourceId"].ToString());

            var deserializedEvent = Activator.CreateInstance(eventType, aggId) as IEvent;

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
                      propertyOnEvent.PropertyType.Equals(typeof(System.Guid)) &&
                      dbObject[key].GetType().Equals(typeof(string));

                if (propertyOnEventIsGuidAndDbObjectPropertyIsString)
                {
                    var parsedGuid = Guid.Parse(dbObject[key].ToString());
                    propertyOnEvent.SetValue(deserializedEvent, parsedGuid, new object[] { });
                }
            }

            return deserializedEvent;
        }
    }
}