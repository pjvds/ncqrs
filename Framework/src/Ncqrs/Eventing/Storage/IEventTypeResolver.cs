using System;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// Resolves types for event names.
    /// </summary>
    /// <remarks>
    /// This is used to avoid reliance upon implementation specific names that can change
    /// due to refactoring such as class names and assembly names.
    ///
    /// Instead when used with <see cref="AttributeEventTypeResolver"/> or a custom implementation
    /// it can allow for event names that are more appropriate outside the application (e.g. perhaps
    /// you have an event directory for all your SOA services with a common naming schema).
    /// </remarks>
    /// <seealso cref="AttributeEventTypeResolver"/>
    public interface IEventTypeResolver
    {
        /// <summary>
        /// Resolves the type for a specified event name.
        /// </summary>
        /// <param name="eventName">The event name to find the type for.</param>
        /// <returns>The type for the event.</returns>
        /// <remarks>
        /// This is used when loading an event from the event store to find the concrete type
        /// for a given event based upon its name.
        /// 
        /// If the event will be converted by <see cref="IPropertyBagConverter.Convert(PropertyBag)"/> then this
        /// would be the final type that the converter returns, i.e. the target type registered with
        /// <see cref="IPropertyBagConverter.AddPostConversion"/>.
        /// </remarks>
        Type ResolveType(string eventName);

        /// <summary>
        /// Gets the event name for a given event type.
        /// </summary>
        /// <param name="type">The event's type to get the name of.</param>
        /// <returns>The event type's name.</returns>
        /// <remarks>
        /// This name will be used when storing the event and used with <see cref="ResolveType"/>
        /// when loading the event.
        /// </remarks>
        string EventNameFor(Type type);
    }
}
