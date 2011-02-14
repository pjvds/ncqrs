using System;

namespace Ncqrs.Eventing.Storage.MongoDB
{
    public class MongoCommit
    {
        public Guid CommitId { get; set; }
        public Guid EventSourceId { get; set; }
        public long FromVersion { get; set; }
        public long ToVersion { get; set; }
        public bool Processed { get; set; }
        public Guid[] Events { get; set; }
    }
}
