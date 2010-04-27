using System;
using Microsoft.WindowsAzure.StorageClient;

namespace Ncqrs.Eventing.Storage.WindowsAzure
{
    internal class SourcedEventEntity : TableServiceEntity
    {
        public int Sequence
        {
            get;
            set;
        }

        public SourcedEventEntity()
        {
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
