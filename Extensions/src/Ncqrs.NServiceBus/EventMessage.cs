using System;
using Ncqrs.Eventing;
using Ncqrs.Eventing.ServiceModel.Bus;
using NServiceBus;
using IEvent = NServiceBus.IEvent;
namespace Ncqrs.NServiceBus
{
    

    [Serializable]
    public class EventMessage : IEventMessage
    {
        public EventMessage() { }

        public EventMessage(IPublishableEvent evnt)
        {
            EventIdentifier = evnt.EventIdentifier;
            EventTimeStamp = evnt.EventTimeStamp;
            EventVersion = evnt.EventVersion;
            EventSourceId = evnt.EventSourceId;
            EventSequence = evnt.EventSequence;
            CommitId = evnt.CommitId;
            Payload = evnt.Payload;
        }
        
        public Guid EventIdentifier { get; set; }
        public DateTime EventTimeStamp { get; set; }
        public Version EventVersion { get; set; }
        public Guid EventSourceId { get; set; }
        public long EventSequence { get; set; }
        public Guid CommitId { get; set; }
        public object Payload { get; set; }
    }

    public interface IEventMessage : IEvent, IPublishableEvent { }
}