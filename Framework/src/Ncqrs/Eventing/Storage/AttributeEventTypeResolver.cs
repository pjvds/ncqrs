using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// This resolves event using attributes to specify their names.
    /// </summary>
    /// <remarks>
    /// This uses <see cref="EventNameAttribute"/> and <see cref="EventNameAliasAttribute"/> to determine
    /// an event's name.
    /// 
    /// All events must first be registered with this resolver using <see cref="AddEvent"/> or
    /// <see cref="AddAllEventsInAssembly"/>.
    /// 
    /// All registered events must have an <see cref="EventNameAttribute"/>. If an event has had its
    /// name changed at some point it can also have multiple <see cref="EventNameAliasAttribute"/>s to allow
    /// deserilization of events using an previous name.
    /// </remarks>
    public class AttributeEventTypeResolver : IEventTypeResolver
    {
        private readonly Dictionary<Type, string> _eventNames;
        private readonly Dictionary<string, Type> _eventTypes;


        public AttributeEventTypeResolver()
        {
            _eventNames = new Dictionary<Type, string>();
            _eventTypes = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
        }


        public Type ResolveType(string eventName)
        {
            if (!_eventTypes.ContainsKey(eventName)) {
                var message = string.Format("Could not find a type for event '{0}'", eventName);
                throw new ArgumentOutOfRangeException("eventName", eventName, message);
            }
            return _eventTypes[eventName];
        }

        public string EventNameFor(Type type)
        {
            if (!_eventNames.ContainsKey(type)) {
                var message = string.Format("Could not find a type for event '{0}'", type);
                throw new ArgumentOutOfRangeException("type", type, message);
            }
            return _eventNames[type];
        }


        /// <summary>
        /// Adds the specified type to the resolver.
        /// </summary>
        /// <param name="type">The type to add.</param>
        /// <remarks>
        /// This will use the attributes <see cref="EventNameAttribute"/> and <see cref="EventNameAliasAttribute"/>
        /// to determine the event's name.
        /// </remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <value>null</value>.</exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="type"/> does not inherit <see cref="IEvent"/>
        /// 
        /// or
        /// 
        /// <paramref name="type"/> does not have an <see cref="EventNameAttribute"/>
        /// 
        /// or
        /// 
        /// another event type is already registered using <paramref name="type"/>'s name (<see cref="EventNameAttribute"/>)
        /// 
        /// or
        /// 
        /// another event type is already registered using one of <paramref name="type"/>'s aliases (<see cref="EventNameAliasAttribute"/>)
        /// </exception>
        public void AddEvent(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type cannot be null");

            if (_eventNames.ContainsKey(type))
                return;

            var name = FindNameForEvent(type);
            var aliases = FindAliasesForEvent(type);

            if (name.Length == 0) {
                string message = string.Format("Type {0} does not have a name", type);
                throw new ArgumentException(message);
            }

            ThrowIfNameExists(type, name);
            foreach (var alias in aliases)
                ThrowIfNameExists(type, alias);

            _eventNames.Add(type, name);
            _eventTypes.Add(name, type);
            foreach (var alias in aliases)
                _eventTypes.Add(alias, type);
        }

        /// <summary>
        /// This will add all the public <see cref="IEvent"/>s in a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to search for <see cref="IEvent"/>s</param>
        /// <exception cref="ArgumentException">
        /// If any of the types do not not have an <see cref="EventNameAttribute"/>
        /// 
        /// or
        /// 
        /// more than one event has the same name (<see cref="EventNameAttribute"/>)
        /// 
        /// or
        /// 
        /// an event has an alias that is used by another event (<see cref="EventNameAliasAttribute"/>)
        /// </exception>
        public void AddAllEventsInAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(x => typeof(IEvent).IsAssignableFrom(x)))
                AddEvent(type);
        }


        private void ThrowIfNameExists(Type type, string name)
        {
             if (_eventTypes.ContainsKey(name)) {
                Type otherType = _eventTypes[name];
                string message = string.Format("Could not add event '{0}' for type '{1}' as the type '{2}' is already using this name.", name, type, otherType);
                throw new ArgumentException(message, "type");
            }
        }

        private static string FindNameForEvent(Type type)
        {
            var names = (EventNameAttribute[]) type.GetCustomAttributes(typeof(EventNameAttribute), false);

            if (names.Length == 0) {
                var message = string.Format("No name found for event {0}, specify an EventNameAttribute.", type);
                throw new ArgumentException(message);
            }

            if (names.Length > 1) {
                var message = string.Format("Multiple names found on event {0}, use EventNameAliasAttribute instead.", type);
                throw new ArgumentException(message);
            }

            return names[0].Name;
        }

        private static IEnumerable<string> FindAliasesForEvent(Type type)
        {
            var names = (EventNameAliasAttribute[]) type.GetCustomAttributes(typeof(EventNameAliasAttribute), false);
            return names.Select(x => x.Name);
        }
    }

    ///<summary>
    /// This specifies the name of an event.
    ///</summary>
    /// <remarks>
    /// This attribute is NOT inherited as each each event type MUST have a different name (this includes aliases).
    /// 
    /// This name is used when serializing and de-serializing an event.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EventNameAttribute : Attribute
    {
        public EventNameAttribute(string name)
        {
            Name = name == null ? "" : name.Trim();
        }

        public string Name { get; private set; }
    }

    ///<summary>
    /// This specifies an alias for an event.
    ///</summary>
    /// <remarks>
    /// This attribute is NOT inherited as each each event type MUST have a different name (this includes aliases).
    /// 
    /// This name is only used when de-serializing an event.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class EventNameAliasAttribute : Attribute
    {
        public EventNameAliasAttribute(string name)
        {
            Name = name == null ? "" : name.Trim();
        }

        public string Name { get; private set; }
    }
}
