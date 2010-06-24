using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.Serialization
{
    public interface IEventTranslator<T>
    {
        StoredEvent<JObject> TranslateToCommon(StoredEvent<T> obj);
        StoredEvent<T> TranslateToRaw(StoredEvent<JObject> obj);
    }
}
