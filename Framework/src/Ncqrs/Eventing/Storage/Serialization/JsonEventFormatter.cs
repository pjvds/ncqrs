using Ncqrs.Eventing.Sourcing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.Serialization
{
    public class JsonEventFormatter : IEventFormatter<JObject>
    {
        private readonly IEventTypeResolver _typeResolver;
        private readonly JsonSerializer _serializer;
        private readonly JsonSerializerSettings _serializerSettings;

        public JsonEventFormatter(IEventTypeResolver typeResolver)
        {
            _typeResolver = typeResolver;
            _serializer = new JsonSerializer();
        }

        public ISourcedEvent Deserialize(StoredEvent<JObject> obj)
        {
            var eventType = _typeResolver.ResolveType(obj.EventName);
            var reader = obj.Data.CreateReader();
            return (ISourcedEvent) _serializer.Deserialize(reader, eventType);
        }

        public StoredEvent<JObject> Serialize(ISourcedEvent theEvent)
        {
            var eventName = _typeResolver.EventNameFor(theEvent.GetType());
            var data = JObject.FromObject(theEvent, _serializer);

            StoredEvent<JObject> obj = new StoredEvent<JObject>(
                theEvent.EventIdentifier,
                theEvent.EventTimeStamp,
                eventName,
                theEvent.EventVersion,
                theEvent.EventSourceId,
                theEvent.EventSequence,
                data);

            data.Remove("EventIdentifier");
            data.Remove("EventTimeStamp");
            data.Remove("EventName");
            data.Remove("EventVersion");
            data.Remove("EventSourceId");
            data.Remove("EventSequence");

            return obj;
        }
    }
}
