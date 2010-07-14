using System;
using System.Linq;
using System.Reflection;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    public class AttributePropertyMappingStrategy
    {
        public PropertyToParameterMappingInfo[] GetMappedProperties(Type target)
        {
            // TODO: At support for both: exclude and include strategy.
            return target.GetProperties().Where
                (
                    p => !p.IsDefined(typeof (ExcludeInMappingAttribute), false)
                ).Select(FromPropertyInfo).ToArray();
        }

        private PropertyToParameterMappingInfo FromPropertyInfo(PropertyInfo prop)
        {
            int? ordinal = null;
            string name = prop.Name;

            var attr = (ParameterAttribute)prop.GetCustomAttributes(typeof(ParameterAttribute), false).FirstOrDefault();

            if(attr != null)
            {
                ordinal = attr.Ordinal;
                name = attr.Name ?? name;
            }

            return new PropertyToParameterMappingInfo(ordinal, name, prop);
        }
    }
}
