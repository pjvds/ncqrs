using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Reflection;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Sourcing.Mapping
{
    /// <summary>
    /// An internal event handler mapping strategy that creates event handlers based on mapping that is done by lambdas.
    /// <remarks>
    /// If u inherit from the <see cref="AggregateRootMappedWithExpressions"/> u must implement the method
    /// InitializeEventHandlers(); Inside this method u can define the mapping between an event and an event source method
    /// in a strongly typed fashion.
    /// <code>
    /// public class Foo : AggregateRootMappedWithExpressions
    /// {
    ///     public override void InitializeEventHandlers()
    ///     {
    ///         Map{SomethingHappenedEvent}().ToHandler(x => SomethingHasHappened(x));
    ///     }
    ///     
    ///     public void SomethingHasHappened(ISourcedEvent e)
    ///     {}
    /// }
    /// 
    /// public class SomethingHappenedEvent : ISourcedEvent
    /// {}
    /// </code>
    /// </remarks>
    /// </summary>
    public class ExpressionBasedEventHandlerMappingStrategy : IEventHandlerMappingStrategy
    {
        /// <summary>
        /// Gets the event handlers from aggregate root based on the given mapping.
        /// </summary>
        /// <param name="target">The aggregate root.</param>
        /// <see cref="ExpressionBasedEventHandlerMappingStrategy"/>
        /// <returns>All the <see cref="ISourcedEventHandler"/>'s created based on the given mapping.</returns>
        public IEnumerable<ISourcedEventHandler> GetEventHandlers(object target)
        {
            Contract.Requires<ArgumentNullException>(target != null, "The target cannot be null.");

            if(!(target is AggregateRootMappedWithExpressions))
                throw new ArgumentException("aggregateRoot need to be of type AggregateRootMappedWithExpressions to be used in a ExpressionBasedEventHandlerMappingStrategy.");

            var handlers = new List<ISourcedEventHandler>();

            foreach (ExpressionHandler mappinghandler in ((AggregateRootMappedWithExpressions)target).MappingHandlers)
            {
                if (mappinghandler.ActionMethodInfo.IsStatic)
                {
                    var message = String.Format("The method {0}.{1} could not be mapped as an event handler, since it is static.", mappinghandler.ActionMethodInfo.DeclaringType.Name, mappinghandler.ActionMethodInfo.Name);
                    throw new InvalidEventHandlerMappingException(message);
                }

                var threshold = mappinghandler.GetType().GetGenericArguments()[0];
                var handler = CreateHandlerForMethod(target, threshold, mappinghandler.ActionMethodInfo, mappinghandler.Exact);
                handlers.Add(handler);
            }

            return handlers;
        }

        /// <summary>
        /// Converts the given method into an <see cref="ISourcedEventHandler"/> object.
        /// </summary>
        /// <param name="eventSource">The event source from which we want to invoke the method.</param>
        /// <param name="method">The method to invoke</param>
        /// <param name="exact"><b>True</b> if we need to have an exact match, otherwise <b>False</b>.</param>
        /// <returns>An <see cref="ISourcedEventHandler"/> that handles the execution of the given method.</returns>
        private static ISourcedEventHandler CreateHandlerForMethod(object eventSource, Type threshold, MethodInfo method, bool exact)
        {
            Action<object> handler = e => method.Invoke(eventSource, new[] { e });
            return new TypeThresholdedActionBasedDomainEventHandler(handler, threshold, method.Name, exact);
        }
    }
}