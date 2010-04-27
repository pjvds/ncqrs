using System;
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
    }
}
