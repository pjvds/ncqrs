using System;
using Newtonsoft.Json;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.RavenDB
{
    public class StoredEvent
    {
        public string Id { get; set; }
        public long EventSequence { get; set; }
        public Guid EventSourceId { get; set; }
        public Guid CommitId { get; set; }
        public Guid EventIdentifier { get; set; }
        public DateTime EventTimeStamp { get; set; }
        public Version Version { get; set; }
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public object Data { get; set; }
    }
}