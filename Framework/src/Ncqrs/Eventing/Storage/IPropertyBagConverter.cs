using System;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// Serializes and deserializes <see cref="ICommand"/> and <see cref="IEvent"/> from/to in a binary format suitable 
    /// for an <see cref="IEventStore"/>.
    /// </summary>
    public interface IPropertyBagConverter
    {
        /// <summary>
        /// Adds a custom converter that resolves differences when deserializing an event or command with
        /// an earlier schema. 
        /// </summary>
        /// <param name="targetType">The type of command or event for which to a add custom post conversion.</param>
        /// <param name="converter">The converter object that will resolve the differences.</param>
        void AddPostConversion(Type targetType, IPropertyBagPostConverter converter);

        /// <summary>
        /// Converts a event into a property bag.
        /// </summary>
        PropertyBag Convert(object obj);

        /// <summary>
        /// Converts a command or event stored as a property bag back to its object representation.
        /// </summary>
        /// <remarks>
        /// If a post conversion was registered using <see cref="AddPostConversion"/>, it will be invoked after
        /// the default conversion has completed. Moreover, the actual type created can be overridden by
        /// providing a custom <see cref="IEventTypeResolver"/> through the <see cref="TypeResolver"/> property.
        /// </remarks>
        object Convert(PropertyBag propertyBag);
    }
}
