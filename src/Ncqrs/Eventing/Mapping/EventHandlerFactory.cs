using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Ncqrs.Eventing.Mapping
{
    public class EventHandlerFactory
    {
        public IEnumerable<KeyValuePair<Type, IEventHandler>> CreateHandlers(MappedEventSource eventSource)
        {
            if (eventSource == null) throw new ArgumentNullException("eventSource");

            var eventSourceType = eventSource.GetType();
            foreach (var method in eventSourceType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (IsMarkedAsEventHandler(method))
                {
                    if (method.IsStatic) // Handlers are never static. Since they need to update the internal state of an eventsource.
                    {
                        // TODO: Throw exception.
                        throw new InvalidOperationException();
                    }
                    if (NumberOfParameters(method) != 1) // The method should only have one parameter.
                    {
                        // TODO: Throw exception.
                        throw new InvalidOperationException();
                    }
                    if (!typeof(IEvent).IsAssignableFrom(FirstParameterType(method))) // The parameter should be an IEvent.
                    {
                        // TODO: Throw exception.
                        throw new InvalidOperationException();
                    }

                    // A method copy is needed because the method variable
                    // itself will change in the next iteration.
                    MethodInfo methodCopy = method;
                    var eventType = methodCopy.GetParameters().First().ParameterType;

                    // TODO: Add validation for given event 'e' instance (e.q. is the type correct?).
                    var handler = new ActionBasedEventHandler((e) => methodCopy.Invoke(eventSource, new object[] { e }));

                    // TODO: Validate that eventType is a "end"-type of IEvent.
                    yield return new KeyValuePair<Type, IEventHandler>(eventType, handler);
                }
            }
        }

        private static Boolean IsMarkedAsEventHandler(MethodInfo target)
        {
            if (target == null) throw new ArgumentNullException("target");

            var attributeType = typeof(EventHandlerAttribute);
            return target.GetCustomAttributes(attributeType, false).Length > 0;
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