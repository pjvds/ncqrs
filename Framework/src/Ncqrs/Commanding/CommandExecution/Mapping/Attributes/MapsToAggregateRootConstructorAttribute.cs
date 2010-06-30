using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Attributes
{
    /// <summary>
    /// Defines that the command maps directly to a constructor on an aggregate root.
    /// </summary>
    public class MapsToAggregateRootConstructorAttribute : CommandMappingAttribute
    {
        private Type _type;

        /// <summary>
        /// Get the full qualified type name of the target aggregate root.
        /// </summary>
        public String TypeName
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the type of the target aggregate root.
        /// </summary>
        public Type Type
        {
            get
            {
                //delay resolving the type from type name to avoid potentially
                //loading another assembly whilst holding class loader locks.
                if (_type == null)
                    _type = Type.GetType(TypeName, true);
                return _type;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapsToAggregateRootConstructorAttribute"/> class.
        /// </summary>
        /// <param name="typeName">The full qualified name of the type of the aggregate root.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>typeName</i> is null or emtpy.</exception>
        public MapsToAggregateRootConstructorAttribute(String typeName)
        {
            if(String.IsNullOrEmpty(typeName)) throw new ArgumentNullException(typeName);

            TypeName = typeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapsToAggregateRootConstructorAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of the aggregate root.</param>
        /// <exception cref="ArgumentNullException">Thrown when <i>type</i> is null.</exception>
        public MapsToAggregateRootConstructorAttribute(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            _type = type;
            TypeName = type.AssemblyQualifiedName;
        }

        public override ICommandExecutor<TCommand> CreateExecutor<TCommand>()
        {
            var commandType = typeof (TCommand);
            ValidateCommandType(commandType);
        }

        private ConstructorInfo GetMatchingConstructor(Type commandType)
        {
            var propertiesToMap = GetPropertiesToMap(commandType);
            var mappedProps = new PropertyInfo[propertiesToMap.Count];
            var potentialCtorTargets = GetProtentialsTargetsOnCount(Type, propertiesToMap.Count);

            if(potentialCtorTargets.Count == 0)
            {
                var msg = string.Format("Command {0} contains {1} to map, but the target "+
                                        "{2} does not contain any ctor that accepts {1} parameters.",
                                        commandType.FullName, propertiesToMap.Count, TypeName);
                throw new CommandMappingException(msg);
            }

            AddOrdinalMappedProperties(mappedProps, propertiesToMap);
            AddNameMappedProperties(potentialCtorTargets, mappedProps, propertiesToMap);
        }

        private List<ConstructorInfo> GetProtentialsTargetsOnCount(Type targetType, int parameterCount)
        {
            var all = BindingFlags.Public | BindingFlags.NonPublic;
            return targetType.GetConstructors(all).Where
            (
                c => c.GetParameters().Length == parameterCount
            ).ToList();
        }

        private List<PropertyInfo> GetPropertiesToMap(Type commandType)
        {
            // TODO: At support for both: exclude and include strategy.
            return commandType.GetProperties().Where
            (
                p => p.IsDefined(typeof (ExcludeInMappingAttribute), false)
            ).ToList();
        }

        private void AddOrdinalMappedProperties(PropertyInfo[] mappedProps, List<PropertyInfo> propertiesToMap)
        {
            for (int i = 0; i < propertiesToMap.Count; i++)
            {
                var prop = propertiesToMap[i];
                var attr = GetParameterAttribute(prop);

                if(attr != null && attr.Ordinal.HasValue)
                {
                    // TODO: Throw ordinal out of range exception if needed.
                    int idx = attr.Ordinal.Value - 1;

                    if (mappedProps[idx] != null)
                        throw new ApplicationException(); // TODO: Throw if already mapped.

                    mappedProps[idx] = prop;
                    
                    // Remove property since we mapped it and decrease index 
                    // variables so that the next property will also be mapped.
                    propertiesToMap.RemoveAt(i);
                    i--;
                }
            }
        }

        private ParameterAttribute GetParameterAttribute(PropertyInfo prop)
        {
            return (ParameterAttribute) prop.GetCustomAttributes(typeof (ParameterAttribute), false).FirstOrDefault();
        }

        private void ValidateCommandType(Type mappedCommandType)
        {
            bool containsThisAttribute = mappedCommandType.IsDefined(this.GetType(), false);

            if(!containsThisAttribute) throw new ArgumentException("The given command type does not contain "+
                                                                   "MapsToAggregateRootConstructorAttribute.",
                                                                   "mappedCommandType");
        }
    }
}
