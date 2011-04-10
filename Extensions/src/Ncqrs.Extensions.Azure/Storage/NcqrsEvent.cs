using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Ncqrs.Eventing;
using Ncqrs.Extensions.Azure;

namespace Ncqrs.Extensions.Azure.Storage
{
    public class NcqrsEvent : TableServiceEntity
    {
        // TODO: 628426 10 Apr 2011 - Use CommitId as designed
        public Guid CommitId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public long Sequence { get; set; }
        public string Data { get; set; }
        
        /// <summary>
        /// Parameterless constructor for Azure Storage deserializer
        /// </summary>
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
