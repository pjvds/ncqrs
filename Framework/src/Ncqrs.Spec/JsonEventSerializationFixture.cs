using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Spec
{
    public abstract class JsonEventSerializationFixture<T>
        : EventSerializationFixture<T>
    {

        public JsonEventSerializationFixture()
        {
            _formatter = BuildFormatter();
        }

        protected string EventName { get; private set; }

        private IEventFormatter<JObject> _formatter;

        protected override string Serialize(T @event)
        {
            string eventName;
            var serializedEvent = _formatter.Serialize(@event, out eventName);
            EventName = eventName;
            return Translate(serializedEvent);
        }

        protected override T Deserialize(string serializedEventData)
        {
            var data = Translate(serializedEventData);
            return (T)_formatter.Deserialize(data, EventName);
        }

        protected virtual IEventTypeResolver BuildEventTypeResolver()
        {
            return new SimpleEventTypeResolver();
        }

        protected virtual IEventFormatter<JObject> BuildFormatter()
        {
            var resolver = BuildEventTypeResolver();
            return new JsonEventFormatter(resolver);
        }

        protected virtual string Translate(JObject jObject)
        {
            return jObject.ToString(Formatting.None, new IsoDateTimeConverter());
        }

        protected virtual JObject Translate(string data)
        {
            return JObject.Parse(data);
        }

    }

}
