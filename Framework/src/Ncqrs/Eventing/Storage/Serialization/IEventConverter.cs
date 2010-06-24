using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.Serialization
{
    public interface IEventConverter
    {
        void Upgrade(StoredEvent<JObject> theEvent);
    }
}
