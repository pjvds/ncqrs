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

            var match = GetMatchingConstructor(commandType);
            return new AggregateRootCreationCommandExecutor<TCommand>((c) =>
            {
                var parameter = match.Item2.Select(p => p.GetValue(c, null));
                match.Item1.Invoke(parameter.ToArray());
            });
        }

        private Tuple<ConstructorInfo, PropertyInfo[]> GetMatchingConstructor(Type commandType)
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

            MakeSureAllPropertiesToMapOnNameHaveUniqueNames(propertiesToMap);
            MakeSureAllPropertieOrdinalsAreUnique(propertiesToMap);

            FilterCtorTargetsOnMappedProperties(potentialCtorTargets, mappedProps);

            if(potentialCtorTargets.Count == 0)
            {
                // TODO: Throw proper ex.
                throw new CommandMappingException("No ctor on "+TypeName+" found that matches the mapping.");
            }

            var matches = FilterCtorTargetsOnNameMappedProperties(potentialCtorTargets, mappedProps, propertiesToMap);
        
            if(matches.Count()==0)
            {
                if (potentialCtorTargets.Count == 0)
                {
                    // TODO: Throw proper ex.
                    throw new CommandMappingException("No ctor on " + TypeName + " found that matches the mapping.");
                }
            }
            else if(matches.Count()>1)
            {
                if (potentialCtorTargets.Count == 0)
                {
                    // TODO: Throw proper ex.
                    throw new CommandMappingException("Multi ctor on " + TypeName + " found that matches the mapping.");
                }
            }

            return matches.Single();
        }

        private void MakeSureAllPropertieOrdinalsAreUnique(List<PropertyInfo> propertiesToMap)
        {
            var query = from p in propertiesToMap
                        let attr = GetParameterAttribute(p)
                        where attr != null
                        group p by attr.Ordinal
                        into g
                        where g.Count() > 1
                        select g.First();

            if(query.Count() > 0)
            {
                var firstDuplicate = query.First();

                throw new CommandMappingException("Cannot map multiple properties with the same name " + firstDuplicate.Name + ".");
            }
        }

        private void MakeSureAllPropertiesToMapOnNameHaveUniqueNames(List<PropertyInfo> propertiesToMap)
        {
            var query = from p in propertiesToMap
                        group p by GetParameterName(p)
                        into g
                        where g.Count() > 1
                        select g.First();

            if (query.Count() > 0)
            {
                var firstDuplicate = query.First();
                // TODO: Better exception.)
                throw new CommandMappingException("Cannot map multiple properties with the same name " + firstDuplicate.Name +
                                                  ".");
            }
        }

        private List<Tuple<T, PropertyInfo[]>> FilterCtorTargetsOnNameMappedProperties<T>(List<T> potentialTargets, PropertyInfo[] mappedProps, List<PropertyInfo> propertiesToMap)
            where T : MethodBase
        {
            var result = new List<Tuple<T, PropertyInfo[]>>();

            foreach(var method in potentialTargets)
            {
                var parameters = method.GetParameters();
                var mapped = new List<PropertyInfo>(mappedProps);
                var toMap = propertiesToMap.Clone();

                for (int i = 0; i < parameters.Length; i++)
                {
                    var prop = mapped[i];
                    var param = parameters[i];

                    if (prop == null)
                    {
                        var matchedOnName = toMap.SingleOrDefault
                        (
                            p => GetParameterName(p).Equals(param.Name, StringComparison.InvariantCultureIgnoreCase)
                        );

                        if(matchedOnName != null)
                        {
                            mapped[i] = matchedOnName;
                            toMap.Remove(matchedOnName);
                        }
                    }
                }

                if(!mapped.Exists(p => p == null))
                {
                    result.Add(new Tuple<T, PropertyInfo[]>(method, mapped.ToArray()));
                }
            }

            return result;
        }

        private string GetParameterName(PropertyInfo prop)
        {
            var mapping = GetParameterAttribute(prop);

            if(mapping != null && string.IsNullOrEmpty(mapping.Name))
            {
                return mapping.Name;
            }
            else
            {
                return prop.Name;
            }
        }

        private void FilterCtorTargetsOnMappedProperties(List<ConstructorInfo> potentialCtorTargets, PropertyInfo[] mappedProps)
        {
            potentialCtorTargets.RemoveAll(ctor =>
            {
                bool remove = false;
                var parameters = ctor.GetParameters();

                for (int i = 0; i < mappedProps.Length; i++)
                {
                    var prop = mappedProps[i];
                    var param = parameters[i];

                    if(prop != null)
                    {
                        bool isAssignable = param.ParameterType.IsAssignableFrom(prop.PropertyType);
                        remove = !isAssignable;
                    }
                }

                return remove;
            });
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
            return (ParameterAttribute) prop.GetCustomAttributes(typeof (ParameterAttribute), false).SingleOrDefault();
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
