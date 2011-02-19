using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Ncqrs.Eventing.Sourcing.Mapping
{
    /// <summary>
    /// An internal event handler mapping strategy that creates event handlers based on mapping that
    /// is done by attributes.
    /// <remarks>Use the <see cref="EventHandlerAttribute"/> to mark event handler methods as an event handler. You can only
    /// mark methods that following rules:
    /// <list type="number">
    /// <item><description>The method should be an instance method (no static).</description></item>
    /// <item><description>It should accept 1 parameter.</description></item>
    /// <item><description>The parameter should be, or inherited from, the <see cref="ISourcedEvent"/> class.</description></item>
    /// <item><description>The method should be marked with the <see cref="EventHandlerAttribute"/>.</description></item>
    /// </list>
    /// <code>public class Foo : AggregateRootMappedWithAttributes
    /// {
    ///     [EventHandler]
    ///     private void onFooEvent(FooEvent eventToHandle)
    ///     {
    ///         // ...
    ///     }
    /// }</code>
    /// </remarks>
    /// </summary>
    public class AttributeBasedEventHandlerMappingStrategy : IEventHandlerMappingStrategy
    {
        /// <summary>
        /// Gets the event handlers from aggregate root based on attributes.
        /// </summary>
        /// <param name="target">The aggregate root.</param>
        /// <see cref="AttributeBasedEventHandlerMappingStrategy"/>
        /// <returns>All the <see cref="IDomainEventHandler"/>'s created based on attribute mapping.</returns>
        public IEnumerable<ISourcedEventHandler> GetEventHandlers(object target)
        {
            Contract.Requires<ArgumentNullException>(target != null, "The target cannot be null.");

            var targetType = target.GetType();
            var handlers = new List<ISourcedEventHandler>();

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

                    var handler = CreateHandlerForMethod(target, method, attribute);
                    handlers.Add(handler);
                }
            }

            return handlers;
        }

        private static ISourcedEventHandler CreateHandlerForMethod(object eventSource, MethodInfo method, EventHandlerAttribute attribute)
        {
            Type firstParameterType = method.GetParameters().First().ParameterType;

            Action<object> handler = e => method.Invoke(eventSource, new[] { e });

            return new TypeThresholdedActionBasedDomainEventHandler(handler, firstParameterType, method.Name, attribute.Exact);
        }

        private static Boolean IsMarkedAsEventHandler(MethodInfo target, out EventHandlerAttribute attribute)
        {
            Contract.Requires<ArgumentNullException>(target != null, "The target cannot be null.");

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
            Contract.Requires<ArgumentNullException>(target != null, "The target cannot be null.");

            return target.GetParameters().Count();
        }

        private static Type FirstParameterType(MethodInfo target)
        {
            Contract.Requires<ArgumentNullException>(target != null, "The target cannot be null.");
            if (NumberOfParameters(target) < 1) throw new ArgumentException("target does not contain parameters.");

            return target.GetParameters().First().ParameterType;
        }
    }
}
