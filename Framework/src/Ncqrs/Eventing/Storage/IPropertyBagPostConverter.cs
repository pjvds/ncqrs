using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// Represents a custom conversion that resolves differences when deserializing an event or command with
    /// an earlier schema. 
    /// </summary>
    public interface IPropertyBagPostConverter
    {
        /// <summary>
        /// Is called after the deserialization process of the <see cref="IPropertyBagConverter"/> has completed
        /// and should be used to resolve differences between earlier versions of a command or event.
        /// </summary>
        /// <param name="target">
        /// A deserialized object.
        /// </param>
        /// <param name="propertyData">
        /// The names and values of the properties of the object when it was serialized. It may include properties
        /// no more available on the current version of the object.
        /// </param>
        void ApplyConversion(object target, Type targetType, IDictionary<string, object> propertyData);
    }
}
