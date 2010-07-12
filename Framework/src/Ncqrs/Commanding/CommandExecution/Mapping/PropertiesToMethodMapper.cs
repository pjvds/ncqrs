using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Commanding.CommandExecution.Mapping
{
    public class PropertiesToMethodMapper
    {
        private static BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static Tuple<ConstructorInfo, PropertyInfo[]> GetConstructor(PropertyToParameterMappingInfo[] sources, Type targetType)
        {
            var potentialTargets = targetType.GetConstructors(All);
            return GetMethodBase(sources, potentialTargets);
        }

        public static Tuple<MethodInfo, PropertyInfo[]> GetMethod(PropertyToParameterMappingInfo[] sources, Type targetType)
        {
            var potentialTargets = targetType.GetMethods(All);
            return GetMethodBase(sources, potentialTargets);
        }

        private static Tuple<TMethodBase, PropertyInfo[]> GetMethodBase<TMethodBase>(PropertyToParameterMappingInfo[] sources, TMethodBase[] potentialTargets)
            where TMethodBase : MethodBase
        {
            var propertiesToMap = new List<PropertyToParameterMappingInfo>(sources);
            var mappedProps = new PropertyInfo[propertiesToMap.Count];
            var targets = new List<MethodBase>(potentialTargets);

            MakeSureAllPropertiesToMapOnNameHaveUniqueNames(propertiesToMap);
            MakeSureAllPropertiesOrdinalsAreUnique(propertiesToMap);

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

        private static void MakeSureAllPropertiesOrdinalsAreUnique(List<PropertyToParameterMappingInfo> propertiesToMap)
        {
            var query = from p in propertiesToMap
                        where p.Ordinal.HasValue
                        group p by p.Ordinal
                        into g
                        where g.Count() > 1
                        select g.First();

            var firstDuplicate = query.FirstOrDefault();
            if (firstDuplicate != null)
            {
                throw new CommandMappingException("Cannot map multiple properties with the same ordinal " + firstDuplicate.TargetName +
                    " (" + firstDuplicate.Ordinal + ").");
            }
        }

        private static void MakeSureAllPropertiesToMapOnNameHaveUniqueNames(List<PropertyToParameterMappingInfo> propertiesToMap)
        {
            var query = from p in propertiesToMap
                        where !p.Ordinal.HasValue
                        group p by p.TargetName
                        into g
                        where g.Count() > 1
                        select g.First();

            var firstDuplicate = query.FirstOrDefault();
            if (firstDuplicate != null)
            {
                // TODO: Better exception.)
                throw new CommandMappingException("Cannot map multiple properties with the same name " + firstDuplicate.TargetName + ".");
            }
        }

        private static List<Tuple<MethodBase, PropertyInfo[]>> FilterCtorTargetsOnNameMappedProperties(List<MethodBase> potentialTargets, PropertyInfo[] mappedProps, List<PropertyToParameterMappingInfo> propertiesToMap)
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
                                p => p.TargetName.Equals(param.Name, StringComparison.InvariantCultureIgnoreCase)
                            );

                        if (matchedOnName != null)
                        {
                            mapped[i] = matchedOnName.Property;
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

        private static void AddOrdinalMappedProperties(PropertyInfo[] mappedProps, List<PropertyToParameterMappingInfo> propertiesToMap)
        {
            for (int i = 0; i < propertiesToMap.Count; i++)
            {
                var prop = propertiesToMap[i];

                if (prop.Ordinal.HasValue)
                {
                    // TODO: Throw ordinal out of range exception if needed.
                    int idx = prop.Ordinal.Value - 1;

                    if (mappedProps[idx] != null)
                        throw new ApplicationException(); // TODO: Throw if already mapped.

                    mappedProps[idx] = prop.Property;

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