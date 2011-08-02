using System;
using System.Diagnostics.Contracts;
using Ncqrs.Eventing.Sourcing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.Serialization
{
    /// <summary>
    /// Serializes events to <see cref="JObject"/>.
    /// </summary>
    public class JsonEventFormatter : IEventFormatter<JObject>
    {
        private readonly IEventTypeResolver _typeResolver;
        private readonly JsonSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEventFormatter"/> class
        /// with a given type resolver.
        /// </summary>
        /// <param name="typeResolver">The <see cref="IEventTypeResolver"/> to use
        /// when resolving event types/names.</param>
        /// <exception cref="ArgumentNullException"><paramref name="typeResolver"/> is <value>null</value>.</exception>
        public JsonEventFormatter(IEventTypeResolver typeResolver)
        {
            Contract.Requires<ArgumentNullException>(typeResolver != null, "typeResolver");

            _typeResolver = typeResolver;
            _serializer = new JsonSerializer();
        }

        public object Deserialize(JObject obj, string eventName)
        {
            var eventType = _typeResolver.ResolveType(eventName);
            var reader = obj.CreateReader();
            var theEvent = _serializer.Deserialize(reader, eventType);
            return theEvent;
        }

        public JObject Serialize(object theEvent, out string eventName)
        {
            eventName = _typeResolver.EventNameFor(theEvent.GetType());
            return JObject.FromObject(theEvent, _serializer);
        }
    }
}
