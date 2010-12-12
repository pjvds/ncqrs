namespace Ncqrs.Messaging
{
    public interface ISetMessageDestination
    {
        ISetMessageRequirements Endpoint(string receiverId);
    }
}