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

        public object Deserialize(StoredEvent<JObject> obj)
        {
            var eventType = _typeResolver.ResolveType(obj.EventName);
            var reader = obj.Data.CreateReader();
            var theEvent = _serializer.Deserialize(reader, eventType);
            
            //TODO: WHAT TODO WITH ENTITIES?
            //json deserialization doesn't seem to restore the EntityId for sourced enetity events
            //SetEntityIdIfEntityEvent(obj, theEvent);

            // TODO: What todo with this? InitizeFRom is legacy...
            if(theEvent is ISourcedEvent) { ((ISourcedEvent)theEvent).InitializeFrom(obj);}
            
            return theEvent;
        }

        private static void SetEntityIdIfEntityEvent(StoredEvent<JObject> obj, ISourcedEvent theEvent)
        {
            var entityEvent = theEvent as ISourcedEntityEvent;
            if(entityEvent != null)
            {
                var eventAllowingSettingOfEntityId = entityEvent as IAllowSettingEntityId;
                if (eventAllowingSettingOfEntityId != null)
                {
                    var entityId = new Guid((string)obj.Data["EntityId"]);
                    eventAllowingSettingOfEntityId.SetEntityId(entityId);
                }
            }
        }

        public StoredEvent<JObject> Serialize(Guid eventIdentifier, DateTime eventTimeStamp, Version eventVersion, Guid eventSourceId, long eventSequence, object theEvent)
        {
            var eventName = _typeResolver.EventNameFor(theEvent.GetType());
            var data = JObject.FromObject(theEvent, _serializer);

            StoredEvent<JObject> obj = new StoredEvent<JObject>(
                eventIdentifier,
                eventTimeStamp,
                eventName,
                eventVersion,
                eventSourceId,
                eventSequence,
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
