using System;
using System.Collections.Generic;
using Ncqrs.Eventing.Storage;

namespace AwesomeAppRefactored.Events
{
    public class NameChangedEventPostConverter : IPropertyBagPostConverter
    {
        public void ApplyConversion(object target, Type targetType, IDictionary<string, object> propertyData)
        {
            NameChangedEvent evnt = (NameChangedEvent)target;
            evnt.Name = string.Format("{0} {1}", propertyData["Forename"], propertyData["Surname"]);
        }
    }
}
