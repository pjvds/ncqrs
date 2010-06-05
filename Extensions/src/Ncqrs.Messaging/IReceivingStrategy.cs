namespace Ncqrs.Messaging
{
    public interface IReceivingStrategy
    {
        IncomingMessage Receive(object message);
    }
}