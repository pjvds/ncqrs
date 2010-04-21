using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Mapping
{
    public class AttributeBasedMappingStrategy : IMappingStrategy
    {
        public IEnumerable<IInternalEventHandler> GetEventHandlersFromAggregateRoot(AggregateRoot aggregateRoot)
        {
            Contract.Requires<ArgumentNullException>(aggregateRoot != null, "The aggregateRoot cannot be null.");

            var targetType = aggregateRoot.GetType();
            foreach (var method in targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                EventHandlerAttribute attribute;

                if (IsMarkedAsEventHandler(method, out attribute))
                {
                    if (method.IsStatic) // Handlers are never static. Since they need to update the internal state of an eventsource.
                    {
                        var message = String.Format("The method {0}.{1} could not be mapped as an event handler, since it is static.", method.DeclaringType.Name, method.Name);
                        throw new InvalidEventHandlerMappingException(message);
                    }
                    if (NumberOfParameters(method) != 1) // The method should only have one parameter.
                    {
                        var message = String.Format("The method {0}.{1} could not be mapped as an event handler, since it has {2} parameters where 1 is required.", method.DeclaringType.Name, method.Name, NumberOfParameters(method));
                        throw new InvalidEventHandlerMappingException(message);
                    }
                    if (!typeof(IEvent).IsAssignableFrom(FirstParameterType(method))) // The parameter should be an IEvent.
                    {
                        var message = String.Format("The method {0}.{1} could not be mapped as an event handler, since it the first parameter is not an event type.", method.DeclaringType.Name, method.Name);
                        throw new InvalidEventHandlerMappingException(message);
                    }

                    yield return CreateHandlerForMethod(aggregateRoot, method, attribute);
                }
            }
        }

        private static IInternalEventHandler CreateHandlerForMethod(AggregateRoot aggregateRoot, MethodInfo method, EventHandlerAttribute attribute)
        {
            Type firstParameterType = method.GetParameters().First().ParameterType;

            Action<IEvent> handler = (e) => method.Invoke(aggregateRoot, new object[] {e});

            return new TypeThresholdedActionBasedInternalEventHandler(handler, firstParameterType, attribute.Exact);
        }

        private static Boolean IsMarkedAsEventHandler(MethodInfo target, out EventHandlerAttribute attribute)
        {
            if (target == null) throw new ArgumentNullException("target");

            var attributeType = typeof(EventHandlerAttribute);
            var attributes = target.GetCustomAttributes(attributeType, false);
            if (attributes.Length > 0)
            {
                attribute = (EventHandlerAttribute)attributes[0];
                return true;
            }

            attribute = null;
            return false;
        }

        private static int NumberOfParameters(MethodInfo target)
        {
            if (target == null) throw new ArgumentNullException("target");

            return target.GetParameters().Count();
        }

        private static Type FirstParameterType(MethodInfo target)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (NumberOfParameters(target) < 1) throw new ArgumentException("target does not contain parameters.");

            return target.GetParameters().First().ParameterType;
        }
    }
}
