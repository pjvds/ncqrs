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
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public ISourcedEvent Data { get; set; }
    }
}