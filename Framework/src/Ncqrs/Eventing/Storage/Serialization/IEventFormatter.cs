using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.Serialization
{
    public interface IEventFormatter<T>
    {
        ISourcedEvent Deserialize(StoredEvent<T> obj);
        StoredEvent<T> Serialize(ISourcedEvent theEvent);
    }
}
