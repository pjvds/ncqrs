using System;
using System.Diagnostics.Contracts;

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
    [ContractClass(typeof(IEventTypeResolverContracts))]
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
        /// <exception cref="ArgumentNullException">If <paramref name="eventName"/> is <value>null</value>.</exception>
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
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <value>null</value>.</exception>
        string EventNameFor(Type type);
    }

    [ContractClassFor(typeof(IEventTypeResolver))]
    internal abstract class IEventTypeResolverContracts : IEventTypeResolver
    {
        public Type ResolveType(string eventName)
        {
            Contract.Requires(eventName != null);
            Contract.Ensures(Contract.Result<Type>() != null);
            return default(Type);
        }

        public string EventNameFor(Type type)
        {
            Contract.Requires(type != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return default(string);
        }
    }
}
