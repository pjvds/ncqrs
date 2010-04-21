using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Mapping
{
    // TODO: Add detailed description.
    /// <summary>
    /// A mapping strategy that maps methods as an event handler based on method name and parameter type.
    /// <remarks>
    /// All method that match the following requirements are mapped as an event handler:
    /// <list type="number">
    ///     <item>
    ///         <value>
    ///             Methodname should start with <i>On</i> or <i>on</i>. Like: <i>OnProductAdded</i> or <i>onProductAdded</i>.
    ///         </value>
    ///     </item>
    ///     <item>
    ///         <value>
    ///             The method should only accept one parameter.
    ///         </value>
    ///     </item>
    ///     <item>
    ///         <value>
    ///             The parameter must be of a type that implements the <see cref="IEvent"/> interface.
    ///         </value>
    ///     </item>
    /// </list>
    /// </remarks>
    /// </summary>
    public class ConventionBasedMappingStrategy : IMappingStrategy
    {
        private String _regexPattern = "^(on|On|ON)+";
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<IInternalEventHandler> GetEventHandlersFromAggregateRoot(AggregateRoot aggregateRoot)
        {
            Contract.Requires<ArgumentNullException>(aggregateRoot != null, "The aggregateRoot cannot be null.");
            Contract.Ensures(Contract.Result<IEnumerable<IInternalEventHandler>>() != null, "The result should never be null.");

            var targetType = aggregateRoot.GetType();
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

                Action<IEvent> invokeAction = (e) => methodCopy.Invoke(aggregateRoot, new object[] {e});

                Logger.DebugFormat("Created event handler for method {0} based on convention.", methodCopy.Name);

                yield return new TypeThresholdedActionBasedInternalEventHandler(invokeAction, firstParameterType, true);
            }
        }
    }
}