namespace Ncqrs.Messaging
{
    public interface ISetMessageDestination
    {
        ISetMessageRequirements NamedEndpoint(string receiverId);
    }
}