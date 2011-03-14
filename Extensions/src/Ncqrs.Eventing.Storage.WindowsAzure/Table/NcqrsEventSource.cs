using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.WindowsAzure
{
    public class NcqrsEventSource : Microsoft.WindowsAzure.StorageClient.TableServiceEntity
    {

        public long Version { get; set; }
        public string Name { get; set; }

        public NcqrsEventSource()
        {
        }

        public NcqrsEventSource(IEventSource source)
            : base("Source_" + source.EventSourceId.ToString(),
                "Source_" + source.EventSourceId.ToString())
        {
            Timestamp = DateTime.UtcNow;
            Version = source.Version;
            Name = source.GetType().ToString();
        }

        public NcqrsEventSource(Guid eventSourceId,
            long version,
            string name) :
            base("Source_" + eventSourceId.ToString(),
                "Source_" + eventSourceId.ToString())
        {
            Timestamp = DateTime.UtcNow;
            Version = Version;
            Name = Name;
        }
    }
}
