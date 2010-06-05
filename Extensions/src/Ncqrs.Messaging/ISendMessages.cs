namespace Ncqrs.Messaging
{
    public interface ISendMessages
    {
        void Send(object payload);
    }
}