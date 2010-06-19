using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Ncqrs.Eventing.Storage
{
    /// <summary>
    /// Converts an <see cref="IEvent"/> from/to a flattened structure suitable for storing in an <see cref="IEventStore"/>.
    /// </summary>
    public class PropertyBagConverter : IPropertyBagConverter
    {
        private const BindingFlags PublicInstanceProperties = BindingFlags.Public | BindingFlags.Instance;
        private readonly Dictionary<Type, IPropertyBagPostConverter> _converters = new Dictionary<Type, IPropertyBagPostConverter>();

        /// <summary>
        /// Gets or sets an optional custom class that resolves types that have (re)moved into their current type.
        /// </summary>
        public IPropertyBagTypeResolver TypeResolver
        {
            get;
            set;
        }

        /// <summary>
        /// Adds a custom converter that resolves differences when converting an event or command with
        /// an earlier schema.
        /// </summary>
        /// <param name="targetType">The type of command or event for which to a add custom post conversion.</param>
        /// <param name="converter">The converter object that will resolve the differences.</param>
        public void AddPostConversion(Type targetType, IPropertyBagPostConverter converter)
        {
            _converters[targetType] = converter;
        }

        /// <summary>
        /// Converts a command or event into a property bag.
        /// </summary>
        public PropertyBag Convert(object obj)
        {
            Type type = obj.GetType();
            var document = new PropertyBag(type);

            foreach (PropertyInfo propertyInfo in type.GetProperties(PublicInstanceProperties))
            {
                document.AddPropertyValue(propertyInfo.Name, propertyInfo.GetValue(obj, null));
            }

            return document;
        }

        /// <summary>
        /// Converts a command or event stored as a property bag back to its object representation.
        /// </summary>
        /// <remarks>
        /// If a post conversion was registered using <see cref="AddPostConversion"/>, it will be invoked after
        /// the default conversion has completed. Moreover, the actual type created can be overridden by
        /// providing a custom <see cref="IPropertyBagTypeResolver"/> through the <see cref="TypeResolver"/> property.
        /// </remarks>
        public object Convert(PropertyBag propertyBag)
        {
            Type targetType = GetDestinationType(propertyBag);
            object instance = Activator.CreateInstance(targetType);

            bool allPropertiesInitialized = InitializeInstancePropertiesFrom(propertyBag, instance);
            bool executedPostConversion = InvokePostConverter(instance, propertyBag);

            if (!allPropertiesInitialized && !executedPostConversion)
            {
                throw new SerializationException(
                    "Not all properties of " + propertyBag.Namespace + " could be deserialized");
            }

            return instance;
        }

        private Type GetDestinationType(PropertyBag bag)
        {
            Type destinationType = null;

            if (TypeResolver != null)
            {
                destinationType = TypeResolver.Resolve(bag.TypeName, bag.Namespace, bag.AssemblyName);
            }

            if (destinationType == null)
            {
                destinationType = Type.GetType(bag.AssemblyQualfiedName);
            }

            return destinationType;
        }

        private static bool InitializeInstancePropertiesFrom(PropertyBag bag, object target)
        {
            bool completelyInitialized = true;

            foreach (KeyValuePair<string, object> pair in bag.Properties)
            {
                string propertyName = pair.Key;
                object propertyValue = pair.Value;

                PropertyInfo targetProperty = target.GetType().GetProperty(propertyName);
                if (IsPropertyWritable(targetProperty))
                {
                    completelyInitialized &= SetPropertyValue(target, targetProperty, propertyValue);
                }
            }

            return completelyInitialized;
        }

        private static bool IsPropertyWritable(PropertyInfo propertyInfo)
        {
            return (propertyInfo != null) && (propertyInfo.CanWrite);
        }

        private static bool SetPropertyValue(object instance, PropertyInfo targetProperty, object value)
        {
            try
            {
                if (RequiresConversion(targetProperty, value))
                {
                    value = System.Convert.ChangeType(value, targetProperty.PropertyType);
                }

                targetProperty.SetValue(instance, value, null);

                return true;
            }
            catch (InvalidCastException)
            {
                // If the conversion is not possible, ignore the exception
                return false;
            }
        }

        private static bool RequiresConversion(PropertyInfo targetProperty, object value)
        {
            return targetProperty.PropertyType != value.GetType();
        }

        private bool InvokePostConverter(object instance, PropertyBag bag)
        {
            Type instanceType = instance.GetType();
            if (_converters.ContainsKey(instanceType))
            {
                IPropertyBagPostConverter converter = _converters[instanceType];
                converter.ApplyConversion(instance, instanceType, bag.Properties);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
