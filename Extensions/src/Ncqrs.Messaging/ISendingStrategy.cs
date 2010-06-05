namespace Ncqrs.Messaging
{
    public interface ISendingStrategy
    {
        void Send(OutgoingMessage message);
    }
}