using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Extensions.WindowsAzure.Events.Storage {
    public class EventSourceEntity : TableServiceEntity {
        public EventSourceEntity(Guid eventSource, long version)
            : base(eventSource.ToString(),
            eventSource.ToString()) {
            _version = version;
        }
        public EventSourceEntity() {
        }
        long _version = 0;

        public long Version {
            get { return _version; }
            set { _version = value; }
        }
    }
}
