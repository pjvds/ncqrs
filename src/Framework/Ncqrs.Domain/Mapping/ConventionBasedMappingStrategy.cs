using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using System.Diagnostics.Contracts;

namespace Ncqrs.Domain.Mapping
{
    // TODO: Add detailed description.
    /// <summary>
    /// A mapping strategy that maps methods as an eventhandler based on method name and parameter type.
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
        //private String _regexPattern = "^on.+";
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<Tuple<Type, Action<IEvent>>> GetEventHandlersFromAggregateRoot(AggregateRoot aggregateRoot)
        {
            Contract.Requires<ArgumentNullException>(aggregateRoot != null, "The aggregateRoot cannot be null.");
            Contract.Ensures(Contract.Result<IEnumerable<Tuple<Type, Action<IEvent>>>>() != null, "The result should never be null.");

            var targetType = aggregateRoot.GetType();
            Logger.DebugFormat("Trying to get all event handlers based by convention for {0}.", targetType);

            var methodsToMatch = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var methodMatchedByName = from method in methodsToMatch
                                      //where Regex.IsMatch(method.Name, _regexPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                                      where method.Name.StartsWith("On", StringComparison.InvariantCultureIgnoreCase)
                                      select method;

            Logger.DebugFormat("{0} methods found based on method name on {1}.", methodMatchedByName.Count(), targetType.FullName);

            var methodMatchedByParameterCount = from method in methodMatchedByName
                                                let parameters = method.GetParameters()
                                                where parameters.Length == 1
                                                select method;

            Logger.DebugFormat("{0} methods left based on number of parameters.", methodMatchedByParameterCount.Count());

            var methodMatchedByParameterType = from method in methodMatchedByParameterCount
                                               let parameters = method.GetParameters()
                                               where typeof(IEvent).IsAssignableFrom(parameters[0].ParameterType)
                                               select method;

            Logger.DebugFormat("{0} methods left based on parameter type.", methodMatchedByParameterType.Count());

            var matchedMethods = from method in methodMatchedByParameterType
                                 let noEventHandlerAttributes = method.GetCustomAttributes(typeof(NoEventHandlerAttribute), true)
                                 where noEventHandlerAttributes.Length == 0
                                 select method;

            Logger.DebugFormat("{0} methods left that are not marked as NoEventHandler.", matchedMethods.Count());

            foreach (var method in matchedMethods)
            {
                var eventType = method.GetParameters().First().ParameterType;
                var methodCopy = method;
                Action<IEvent> invokeAction = (e) => methodCopy.Invoke(aggregateRoot, new object[] { e });

                Logger.DebugFormat("Created event handler for method {0} based on convention.", methodCopy.Name);

                yield return Tuple.Create(eventType, invokeAction);
            }
        }
    }
}