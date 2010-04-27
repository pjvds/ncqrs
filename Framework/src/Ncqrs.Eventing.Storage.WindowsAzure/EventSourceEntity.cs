using System;
using System.Diagnostics.Contracts;
using Microsoft.WindowsAzure.StorageClient;

namespace Ncqrs.Eventing.Storage.WindowsAzure
{
    internal class EventSourceEntity : TableServiceEntity
    {
        public long Version
        {
            get;
            set;
        }

        public EventSourceEntity()
        {
        }

        public static EventSourceEntity CreateFromEventSource(IEventSource source)
        {
            Contract.Requires<ArgumentNullException>(source != null, "The source cannot be null.");

            return new EventSourceEntity
            {
                RowKey = source.Id.ToString(),
                PartitionKey = source.GetType().FullName,
                Version = source.Version
            };
        }
    }
}
