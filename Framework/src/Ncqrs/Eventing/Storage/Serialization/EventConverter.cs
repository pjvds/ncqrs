using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Newtonsoft.Json.Linq;

namespace Ncqrs.Eventing.Storage.Serialization
{
    public class EventConverter : IEventConverter
    {
        private Dictionary<string, IEventConverter> _converters;
        private IEventTypeResolver _typeResolver;


        public EventConverter(IEventTypeResolver typeResolver)
        {
            Contract.Requires<ArgumentNullException>(typeResolver != null, "typeResolver cannot be null");

            _converters = new Dictionary<string, IEventConverter>();
            _typeResolver = typeResolver;
        }


        public void Upgrade(StoredEvent<JObject> theEvent)
        {
            IEventConverter converter;
            if (_converters.TryGetValue(theEvent.EventName, out converter))
                converter.Upgrade(theEvent);
        }


        public void AddConverter(Type eventType, IEventConverter converter)
        {
            string name = _typeResolver.EventNameFor(eventType);
            AddConverter(name, converter);
        }

        public void AddConverter(string eventName, IEventConverter converter)
        {
            ThrowIfNameExists(eventName);
            _converters.Add(eventName, converter);
        }


        private void ThrowIfNameExists(string eventName) {
            if (_converters.ContainsKey(eventName)) {
                string message = string.Format("There is already a converter for event '{0}'.", eventName);
                throw new ArgumentException(message, "eventName");
            }
        }
    }
}
