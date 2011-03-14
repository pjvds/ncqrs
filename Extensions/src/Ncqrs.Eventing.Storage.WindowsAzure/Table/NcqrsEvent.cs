using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Ncqrs.Eventing.Storage.WindowsAzure
{
    internal class NcqrsEvent : TableServiceEntity
    {
        public Guid CommitId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public long Sequence { get; set; }
        public string Data { get; set; }
        
        public NcqrsEvent()
        {
        }

        public NcqrsEvent(UncommittedEvent @event) :
            base(@event.EventSourceId.ToString(), @event.EventIdentifier.ToString())
        {
            base.Timestamp = @event.EventTimeStamp;
            Name = @event.Payload.GetType().AssemblyQualifiedName;
            Sequence = @event.EventSequence;
            Version = @event.EventVersion.ToString();

            if (@event.Payload != null)
            {
                Data = Utility.Jsonize(@event.Payload, Name);
            }
        }
    }
}
