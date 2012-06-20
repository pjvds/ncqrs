using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.AWS
{
    public class NcqrsEventSource
    {
        public long Version { get; set; }
        public string Name { get; set; }
        public Guid EventSourceId { get; set; }
        public DateTime Timestamp { get; set; }

        public NcqrsEventSource()
        { }

        public NcqrsEventSource(IEventSource source)
        {
            EventSourceId = source.EventSourceId;
            Timestamp = DateTime.UtcNow;
            Version = source.Version;
            Name = source.GetType().ToString();
        }

        public NcqrsEventSource(Guid eventSourceId,
            long version,
            string name)
        {
            EventSourceId = eventSourceId;
            Timestamp = DateTime.UtcNow;
            Version = version;
            Name = name;
        }
    }
}
