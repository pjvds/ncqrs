namespace Ncqrs.Messaging.NServiceBus
{
    public class NsbLocalReceivingStrategy : IReceivingStrategy
    {
        private readonly IReceivingStrategy _inMemoryLocal = new LocalInMemoryReceivingStrategy();

        public IncomingMessage Receive(object message)
        {
            var localMessage = (LocalMessage) message;
            return _inMemoryLocal.Receive(localMessage.Message);
        }
    }
}