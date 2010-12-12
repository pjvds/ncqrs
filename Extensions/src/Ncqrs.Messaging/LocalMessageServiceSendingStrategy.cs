namespace Ncqrs.Messaging
{
    public class LocalMessageServiceSendingStrategy : ISendingStrategy
    {
        public void Send(OutgoingMessage message)
        {
            var messageService = NcqrsEnvironment.Get<IMessageService>();
            messageService.Process(message);
        }
    }
}