using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ncqrs.Commands;
using Ncqrs.Commands.Attributes;

namespace Ncqrs.CommandHandling.AutoMapping
{
    public class AutoMapperConfiguration
    {
        /// <summary>
        /// Gets all the values for the parameters filled with the values from the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>All the values for the parameters.</returns>
        public object[] GetParameterValues(ICommand command, ParameterInfo[] parameters)
        {
            var buffer = new List<object>();

            foreach (var param in parameters)
            {
                object value = GetPropertyValueBasedOnParameterInfo(command, param);
                buffer.Add(value);
            }

            return buffer.ToArray();
        }

        /// <summary>
        /// Gets the property value from the first matched property. The match is made by the name and the type of the specified parameter.
        /// </summary>
        /// <remarks>The property match is done by the name and type, where the name is matched case insensitive and the type of the 
        /// parameter type should be assignable from the property type.</remarks>
        /// <param name="command">The command.</param>
        /// <param name="parameterInfo">The parameter info.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>command</i> or <i>parameterInfo</i> is null.</exception>
        /// <returns>The value from the first matched property.</returns>
        public object GetPropertyValueBasedOnParameterInfo(ICommand command, ParameterInfo parameterInfo)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (parameterInfo == null) throw new ArgumentNullException("parameterInfo");

            // Initialize result with the default value based on the type.
            var type = parameterInfo.ParameterType;
            object result = type.IsValueType ? Activator.CreateInstance(type) : null;

            // Get all properties that match name of the specified parameter and where the property type
            // is assignable from the parameter type.
            var query = from prop in command.GetType().GetProperties()
                        where prop.Name.Equals(parameterInfo.Name, StringComparison.InvariantCultureIgnoreCase) &&
                              parameterInfo.ParameterType.IsAssignableFrom(prop.PropertyType)
                        select prop;

            // Get the first property, if found.
            var propertyInfo = query.FirstOrDefault();

            // If there is a property found, get the value from it.
            if (propertyInfo != null)
            {
                result = propertyInfo.GetValue(command, null);
            }

            return result;
        }

        /// <summary>
        /// Gets all the propertie from the command that should be used in the auto mapping process.
        /// </summary>
        /// <remarks>All properties marked with the <see cref="ExcludeInMappingAttribute"/> are ignored.</remarks>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>command</i> is null.</exception>
        /// <returns>A result set containing all properties that should be used in the auto mapping process.</returns>
        public IEnumerable<PropertyInfo> GetCommandProperties(ICommand command)
        {
            if(command == null) throw new ArgumentNullException("command");

            var commandType = command.GetType();

            foreach (var prop in commandType.GetProperties())
            {
                var excludeInMappingAttributes = prop.GetCustomAttributes(typeof(ExcludeInMappingAttribute), true);

                if (excludeInMappingAttributes.Count() == 0)
                {
                    yield return prop;
                }
            }
        }
    }
}
