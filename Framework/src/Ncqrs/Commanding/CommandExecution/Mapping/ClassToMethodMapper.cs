using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Commanding.CommandExecution.Mapping
{
    public class ClassToMethodMapper
    {
        private static BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static Tuple<ConstructorInfo, PropertyInfo[]> GetConstructor(Type sourceType, Type targetType)
        {
            var potentialTargets = targetType.GetConstructors(All);
            return GetMethodBase(sourceType, potentialTargets);
        }

        public static Tuple<MethodInfo, PropertyInfo[]> GetMethod(Type sourceType, Type targetType)
        {
            var potentialTargets = targetType.GetMethods(All);
            return GetMethodBase(sourceType, potentialTargets);
        }

        public static Tuple<TMethodBase, PropertyInfo[]> GetMethodBase<TMethodBase>(Type sourceType, TMethodBase[] potentialTargets)
            where TMethodBase : MethodBase
        {
            var propertiesToMap = GetPropertiesToMap(sourceType);
            var mappedProps = new PropertyInfo[propertiesToMap.Count];
            var targets = new List<MethodBase>(potentialTargets);

            MakeSureAllPropertiesToMapOnNameHaveUniqueNames(propertiesToMap);
            MakeSureAllPropertieOrdinalsAreUnique(propertiesToMap);

            // Remove all targets that do no match the parameter count.
            targets.RemoveAll(t=>!HasCorrectParameterCount(t, propertiesToMap.Count));

            if (targets.IsEmpty())
            {
                var msg = string.Format("No target found that accepts {0} parameter(s).",
                                        propertiesToMap.Count);
                throw new CommandMappingException(msg);
            }

            AddOrdinalMappedProperties(mappedProps, propertiesToMap);

            targets.RemoveAll(t => !IsTargetInvokableFromKnownProperties(t, mappedProps));

            if (targets.Count == 0)
            {
                // TODO: Throw proper ex.
                throw new CommandMappingException("No target found that matches the mapping.");
            }

            var matches = FilterCtorTargetsOnNameMappedProperties(targets, mappedProps, propertiesToMap);

            if (matches.Count() == 0)
            {
                // TODO: Throw proper ex.
                throw new CommandMappingException("No ctor on found that matches the mapping.");
            }
            else if (matches.Count() > 1)
            {
                // TODO: Throw proper ex.
                throw new CommandMappingException("Multi ctor on found that matches the mapping.");
            }

            var match = matches.Single();
            return new Tuple<TMethodBase, PropertyInfo[]>((TMethodBase)match.Item1, match.Item2);
        }

        private static void MakeSureAllPropertieOrdinalsAreUnique(List<PropertyInfo> propertiesToMap)
        {
            var query = from p in propertiesToMap
                        let attr = GetParameterAttribute(p)
                        where attr != null
                        group p by attr.Ordinal
                        into g
                        where g.Count() > 1
                        select g.First();

            if (query.Count() > 0)
            {
                var firstDuplicate = query.First();

                throw new CommandMappingException("Cannot map multiple properties with the same name " + firstDuplicate.Name + ".");
            }
        }

        private static void MakeSureAllPropertiesToMapOnNameHaveUniqueNames(List<PropertyInfo> propertiesToMap)
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

        private static List<Tuple<MethodBase, PropertyInfo[]>> FilterCtorTargetsOnNameMappedProperties(List<MethodBase> potentialTargets, PropertyInfo[] mappedProps, List<PropertyInfo> propertiesToMap)
        {
            var result = new List<Tuple<MethodBase, PropertyInfo[]>>();

            foreach (var method in potentialTargets)
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

                        if (matchedOnName != null)
                        {
                            mapped[i] = matchedOnName;
                            toMap.Remove(matchedOnName);
                        }
                    }
                }

                if (!mapped.Exists(p => p == null))
                {
                    result.Add(new Tuple<MethodBase, PropertyInfo[]>(method, mapped.ToArray()));
                }
            }

            return result;
        }

        private static string GetParameterName(PropertyInfo prop)
        {
            var mapping = GetParameterAttribute(prop);

            if (mapping != null && string.IsNullOrEmpty(mapping.Name))
            {
                return mapping.Name;
            }
            else
            {
                return prop.Name;
            }
        }

        private static bool IsTargetInvokableFromKnownProperties(MethodBase target, PropertyInfo[] mappedProps)
        {
            bool isInvokable = true;
            var parameters = target.GetParameters();

            for (int i = 0; i < mappedProps.Length; i++)
            {
                var prop = mappedProps[i];
                var param = parameters[i];

                if (prop != null)
                {
                    bool isAssignable = param.ParameterType.IsAssignableFrom(prop.PropertyType);
                    isInvokable &= isAssignable;
                }
            }

            return isInvokable;
        }

        private static bool HasCorrectParameterCount(MethodBase target, int parameterCount)
        {
            return target.GetParameters().Length == parameterCount;
        }

        private static List<PropertyInfo> GetPropertiesToMap(Type commandType)
        {
            // TODO: At support for both: exclude and include strategy.
            return commandType.GetProperties().Where
                (
                    p => !p.IsDefined(typeof(ExcludeInMappingAttribute), false)
                ).ToList();
        }

        private static void AddOrdinalMappedProperties(PropertyInfo[] mappedProps, List<PropertyInfo> propertiesToMap)
        {
            for (int i = 0; i < propertiesToMap.Count; i++)
            {
                var prop = propertiesToMap[i];
                var attr = GetParameterAttribute(prop);

                if (attr != null && attr.Ordinal.HasValue)
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

        private static ParameterAttribute GetParameterAttribute(PropertyInfo prop)
        {
            return (ParameterAttribute)prop.GetCustomAttributes(typeof(ParameterAttribute), false).SingleOrDefault();
        }
    }
}