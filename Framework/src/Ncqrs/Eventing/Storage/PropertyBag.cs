using System;
using System.Collections.Generic;

namespace Ncqrs.Eventing.Storage
{
    [Serializable]
    public class PropertyBag
    {
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:System.Object" /> class.
        /// </summary>
        public PropertyBag(string eventName)
        {
            EventName = eventName;
        }

        /// <summary>
        ///   Gets or sets the event name of the original object from which this document was constructed.
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        ///   Gets or sets the names and values of all public properties of the original object.
        /// </summary>
        public IDictionary<string, object> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Adds a property value to this bag.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void AddPropertyValue(string propertyName, object value)
        {
            if(propertyName.IsNullOrEmpty()) throw new ArgumentNullException("propertyName");

            _properties.Add(propertyName, value);
        }
    }
}
