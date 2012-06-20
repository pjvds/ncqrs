using System;
using Ncqrs.Eventing.ServiceModel.Bus;

namespace Ncqrs.Eventing.Storage.AWS
{
    internal class NcqrsEvent
    {
        public Guid EventSourceId { get; set; }
        public Guid EventIdentifier { get; set; }
        public Guid CommitId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public long Sequence { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }

        public NcqrsEvent()
        { }

        public NcqrsEvent(IPublishableEvent @event)
        {
            Timestamp = DateTime.UtcNow;
            EventSourceId = @event.EventSourceId;
            EventIdentifier = @event.EventIdentifier;
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
