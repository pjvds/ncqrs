using System;
using NServiceBus;

namespace Ncqrs.Messaging.NServiceBus
{
    public class NsbLocalSendingStrategy : ISendingStrategy
    {
        public void Send(OutgoingMessage message)
        {
            Bus.SendLocal(new LocalMessage {Message = message});
        }

        private static IBus Bus
        {
            get { return NcqrsEnvironment.Get<IBus>(); }
        }
    }
}