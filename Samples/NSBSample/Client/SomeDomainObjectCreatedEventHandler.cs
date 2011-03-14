using System;
using Events;
using Ncqrs.NServiceBus;
using NServiceBus;

namespace Client
{
    public class SomeDomainObjectCreatedEventHandler : IMessageHandler<EventMessage<SomeDomainObjectCreatedEvent>>
    {
        public void Handle(EventMessage<SomeDomainObjectCreatedEvent> message)
        {
            ClientEndpoint.AggregateId = message.Payload.ObjectId;
            Console.WriteLine("Aggregate with ID={0} created", message.Payload.ObjectId);
        }
    }
}