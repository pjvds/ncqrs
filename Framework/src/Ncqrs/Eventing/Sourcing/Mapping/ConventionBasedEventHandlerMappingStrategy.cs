using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Ncqrs.Eventing.Sourcing.Mapping
{
    /// <summary>
    /// A internal event handler mapping strategy that maps methods as an event handler based on method name and parameter type.
    /// <remarks>
    /// All method that match the following requirements are mapped as an event handler:
    /// <list type="number">
    ///     <item>
    ///         <value>
    ///             Method name should start with <i>On</i> or <i>on</i>. Like: <i>OnProductAdded</i> or <i>onProductAdded</i>.
    ///         </value>
    ///     </item>
    ///     <item>
    ///         <value>
    ///             The method should only accept one parameter.
    ///         </value>
    ///     </item>
    ///     <item>
    ///         <value>
    ///             The parameter must be, or inhired from, the <see cref="SourcedEvent"/> class.
    ///         </value>
    ///     </item>
    /// </list>
    /// </remarks>
    /// </summary>
    public class ConventionBasedEventHandlerMappingStrategy : IEventHandlerMappingStrategy
    {
        private String _regexPattern = "^(on|On|ON)+";
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<ISourcedEventHandler> GetEventHandlers(object target)
        {
            Contract.Requires<ArgumentNullException>(target != null, "The target cannot be null.");
            Contract.Ensures(Contract.Result<IEnumerable<ISourcedEventHandler>>() != null, "The result should never be null.");

            var targetType = target.GetType();
            var handlers = new List<ISourcedEventHandler>();

            Logger.DebugFormat("Trying to get all event handlers based by convention for {0}.", targetType);

            var methodsToMatch = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var matchedMethods = from method in methodsToMatch
                                 let parameters = method.GetParameters()
                                 let noEventHandlerAttributes =
                                     method.GetCustomAttributes(typeof(NoEventHandlerAttribute), true)
                                 where
                                     // Get only methods where the name matches.
                                    Regex.IsMatch(method.Name, _regexPattern, RegexOptions.CultureInvariant) &&
                                     // Get only methods that have 1 parameter.
                                    parameters.Length == 1 &&
                                     // Get only methods where the first parameter is an event.
                                    typeof(IEvent).IsAssignableFrom(parameters[0].ParameterType) &&
                                     // Get only methods that are not marked with the no event handler attribute.
                                    noEventHandlerAttributes.Length == 0
                                 select
                                    new { MethodInfo = method, FirstParameter = method.GetParameters()[0] };

            foreach (var method in matchedMethods)
            {
                var methodCopy = method.MethodInfo;
                Type firstParameterType = methodCopy.GetParameters().First().ParameterType;

                Action<IEvent> invokeAction = (e) => methodCopy.Invoke(target, new object[] { e });

                Logger.DebugFormat("Created event handler for method {0} based on convention.", methodCopy.Name);

                var handler = new TypeThresholdedActionBasedDomainEventHandler(invokeAction, firstParameterType, true);
                handlers.Add(handler);
            }

            return handlers;
        }
    }
}