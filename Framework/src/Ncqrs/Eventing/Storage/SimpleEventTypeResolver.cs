using System;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// This resolves types using their assembly qualified name.
    /// </summary>
    /// <remarks>
    /// This is not recommended for production use as it means event names will be affected by changes
    /// to namespaces and class names.
    /// </remarks>
    /// <seealso cref="AttributeEventTypeResolver"/>
    public class SimpleEventTypeResolver : IEventTypeResolver
    {
        public Type ResolveType(string eventName)
        {
            return Type.GetType(eventName, true, false);
        }

        public string EventNameFor(Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}
