using System;
using NServiceBus;

namespace Ncqrs.Messaging.NServiceBus
{
    [Serializable]
    public class LocalMessage : IMessage
    {
        public OutgoingMessage Message { get; set; }
    }
}