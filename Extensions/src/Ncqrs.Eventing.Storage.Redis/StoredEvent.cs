using System;
using Ncqrs.Eventing.Sourcing;
using ServiceStack.DesignPatterns.Model;

namespace Ncqrs.Eventing.Storage.Redis
{
    public class StoredEvent : IHasStringId
    {
        public string Id { get; set; }
        public long EventSequence { get; set; }
        public Guid EventSourceId { get; set; }
        public Type DataType { get; set; }
        public string Data { get; set; }
    }
}