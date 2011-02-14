using System;
using MongoDB.Bson;

namespace Ncqrs.Eventing.Storage.MongoDB
{
    public class MongoEventStream
    {
        public Guid CommitId { get; set; }
        public Guid EventSourceId { get; set; }
        public long FromVersion { get; set; }
        public long ToVersion { get; set; }
        public BsonValue Payload
        {
            get;
            set;
        }
    }
}
