namespace Ncqrs.Messaging
{
    public interface ISetMessageRequirements : ISendMessages
    {
        ISendMessages Ensuring(MessageProcessingRequirements requirements);
    }
}