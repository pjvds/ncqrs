using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ncqrs.Eventing.Sourcing;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Ncqrs.Eventing.Storage.MongoDB
{
    public class MongoDBEventStore : IEventStore
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The default data uri that points to a local Mongo DB.
        /// </summary>
        //protected const string DEFAULT_DATABASE_URI = "mongodb://127.0.0.1:27017/EventStore";

        private readonly MongoDatabase _database;
        private readonly MongoCollection<CommittedEventStream> _eventStreams;

        public MongoDBEventStore(string connectionString)
        {
            _database = MongoDatabase.Create(connectionString);
            _eventStreams = _database.GetCollection<CommittedEventStream>("EventStreams");

            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            _eventStreams.EnsureIndex(
                IndexKeys.Descending("EventSourceId", "FromVersion"),
                IndexOptions.SetName("OptimisticEventSourceConcurrencyIndex").SetUnique(true));
        }

        public CommittedEventStream ReadUntil(Guid id, long? maxVersion)
        {
            var query = Query.EQ("SourceId", id);
                
            // TODO: We can select events above maxversion since a commit can have the correct FromVersion, but contain events higher then max version.
            if(maxVersion.HasValue)
                query = Query.And(query, Query.LTE("FromVersion", maxVersion));

            var streams = _eventStreams.Find(query).SetSortOrder("FromVersion");
            
            return CommittedEventStream.Combine(streams);
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion)
        {
            // TODO: We can select events above maxversion since a commit can have the correct FromVersion, but contain events higher then max version.
            var query = Query.And(Query.EQ("SourceId", id), Query.GTE("FromVersion", minVersion));

            var streams = _eventStreams.Find(query).SetSortOrder("FromVersion");

            return CommittedEventStream.Combine(streams);
        }

        public void Store(UncommittedEventStream eventStream)
        {
            _eventStreams.Save(eventStream);
        }
    }
}