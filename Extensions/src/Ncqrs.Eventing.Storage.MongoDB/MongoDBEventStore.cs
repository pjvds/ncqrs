using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Ncqrs.Eventing.Sourcing;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using Ncqrs.Eventing.Storage.Serialization;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.MongoDB
{
    public class MongoDBEventStore : IEventStore
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The default data uri that points to a local Mongo DB.
        /// </summary>
        protected const string DEFAULT_DATABASE_URI = "mongodb://127.0.0.1:27017/EventStore";

        /// <summary>
        /// The error code
        /// </summary>
        protected const string CONCURRENCY_ERROR_CODE = "E1100";


        private readonly MongoDatabase _database;
        private readonly MongoCollection<MongoCommit> _commits;
        private readonly MongoCollection<BsonDocument> _events;

        private IEventFormatter<JObject> _formatter;
        private IEventTranslator<string> _translator;
        private IEventConverter _converter;


        public MongoDBEventStore(string connectionString = DEFAULT_DATABASE_URI, IEventTypeResolver typeResolver = null, IEventConverter converter = null)
        {
            _database = MongoDatabase.Create(connectionString);
            _commits = _database.GetCollection<MongoCommit>("Commits");
            _events = _database.GetCollection<BsonDocument>("SourcedEvents");

            _converter = converter ?? new NullEventConverter();
            _formatter = new JsonEventFormatter(typeResolver ?? new SimpleEventTypeResolver());
            _translator = new StringEventTranslator();

            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            _commits.EnsureIndex(
                IndexKeys.Ascending("CommitId"),
                IndexOptions.SetName("CommitIdIndex"));

            _commits.EnsureIndex(
                IndexKeys.Ascending("Processed"),
                IndexOptions.SetName("ProcessedIndex"));

            _commits.EnsureIndex(
                IndexKeys.Descending("EventSourceId", "FromVersion"),
                IndexOptions.SetName("OptimisticEventSourceConcurrencyIndex").SetUnique(true));

            _events.EnsureIndex(
                IndexKeys.Ascending("EventIdentifier"),
                IndexOptions.SetName("EventIdentifierIndex"));
        }

        public CommittedEventStream ReadUntil(Guid id, long? maxVersion)
        {
            //var query = Query.EQ("SourceId", id);
                
            //// TODO: We can select events above maxversion since a commit can have the correct FromVersion, but contain events higher then max version.
            //if(maxVersion.HasValue)
            //    query = Query.And(query, Query.LTE("FromVersion", maxVersion));

            //var streams = _events.Find(query).SetSortOrder("FromVersion");
            //var events = streams.Select(strm => strm.Events);           
            //return CommittedEventStream.Combine(streams);
            return null;
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion = 0)
        {
            //// TODO: We can select events above maxversion since a commit can have the correct FromVersion, but contain events higher then max version.
            //var query = Query.And(Query.EQ("SourceId", id), Query.GTE("FromVersion", minVersion));

            //var streams = _eventStreams.Find(query).SetSortOrder("FromVersion");

            //return CommittedEventStream.Combine(streams);
            return null;
        }

        public void Store(UncommittedEventStream eventStream)
        {
            var commit = new MongoCommit
            {
                CommitId = eventStream.CommitId,
                EventSourceId = eventStream.SourceId,
                FromVersion = eventStream.InitialVersion,
                ToVersion = eventStream.Last().EventSequence,
                Events = eventStream.Select(e=>e.EventIdentifier).ToArray(),
                Processed = false
            };

            try
            {
                try
                {
                    SafellyInsertCommit(commit);
                    InsertEvents(eventStream);
                    MarkCommitAsProcessed(commit.CommitId);
                }
                catch
                {
                    RemoveUnprocessedCommit(commit.CommitId);
                    throw;
                }
            }
            catch (MongoSafeModeException ex)
            {
                if (ex.Message.Contains(CONCURRENCY_ERROR_CODE))
                    throw new ConcurrencyException(eventStream.SourceId, -1);
            }
        }

        private void InsertEvents(IEnumerable<UncommittedEvent> events)
        {
            //var docs = events.Select(evnt =>
            //{
            //    ////var document = _formatter.Serialize(evnt.EventIdentifier, evnt.EventTimeStamp, evnt.EventVersion,
            //    ////                                    evnt.EventSourceId, evnt.EventSequence, evnt.Payload);
                
            //    //var raw = _translator.TranslateToRaw(document);
            //    //var bsonDocument = raw.ToBsonDocument();

            //    //return bsonDocument;
            //});

            throw new Exception("Not implemented!");
            //_commits.Insert(docs, SafeMode.True);
        }

        private void RemoveUnprocessedCommit(Guid commitId)
        {
            var query = Query.EQ("CommitId", BsonValue.Create(commitId));

            var commitToRemove = _commits.FindOne(query);

            if (commitToRemove != null)
            {
                RemoveEventsForCommit(commitToRemove);

                _commits.Remove(Query.EQ("CommitId", commitId));
            }
        }

        private void MarkCommitAsProcessed(Guid commitId)
        {
            _commits.Update(Query.EQ("CommitId", BsonValue.Create(commitId)),
                            Update.Push("Processed", BsonValue.Create(true)));
        }

        private void RemoveEventsForCommit(MongoCommit commit)
        {
            _events.Remove(Query.In("EventIdentifier", BsonArray.Create(commit.Events)));            
        }

        private void SafellyInsertCommit(MongoCommit commit)
        {
            try
            {
                var result = _commits.Insert(commit, SafeMode.True);
                _commits.Validate();
            }
            catch (MongoSafeModeException ex)
            {
                if (ex.Message.Contains(CONCURRENCY_ERROR_CODE))
                    throw new ConcurrencyException(commit.EventSourceId, commit.FromVersion);
            }
        }

        private static BsonDocument ToBsonDocument(StoredEvent<string> rawEvent)
        {
            var doc = rawEvent.ToBsonDocument();
            return doc;
        }

       
 
    }
}
