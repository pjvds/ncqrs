using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Ncqrs.Domain.Mapping
{
    /// <summary>
    /// An internal event handler mapping strategy that creates event handlers based on mapping that is done by lambdas.
    /// <remarks>
    /// If u inherit from the <see cref="AggregateRootMappedWithExpressions"/> u must implement the method
    /// InitializeEventHandlers(); Inside this method u can define the mapping between an event and an aggregateroot method
    /// in a strongly typed fashion.
    /// <code>
    /// public class Foo : AggregateRootMappedWithExpressions
    /// {
    ///     public override void InitializeEventHandlers()
    ///     {
    ///         Map<SomethingHappenedEvent>().ToHandler(x => SomethingHasHappened(x));
    ///     }
    ///     
    ///     public void SomethingHasHappened(DomainEvent e)
    ///     {}
    /// }
    /// 
    /// public class SomethingHappenedEvent : DomainEvent
    /// {}
    /// </code>
    /// </remarks>
    /// </summary>
    public class ExpressionBasedDomainEventHandlerMappingStrategy : IDomainEventHandlerMappingStrategy
    {
        /// <summary>
        /// Gets the event handlers from aggregate root based on the given mapping.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root.</param>
        /// <see cref="ExpressionBasedDomainEventHandlerMappingStrategy"/>
        /// <returns>All the <see cref="IDomainEventHandler"/>'s created based on the given mapping.</returns>
        public IEnumerable<IDomainEventHandler> GetEventHandlersFromAggregateRoot(object aggregateRoot)
        {
            Contract.Requires<ArgumentNullException>(aggregateRoot != null, "The aggregateRoot cannot be null.");

            if(!(aggregateRoot is AggregateRootMappedWithExpressions))
            {
                throw new ArgumentException("aggregateRoot need to be of type AggregateRootMappedWithExpressions to be used in a ExpressionBasedDomainEventHandlerMappingStrategy.");
            }

            var handlers = new List<IDomainEventHandler>();

            foreach (ExpressionHandler mappinghandler in ((AggregateRootMappedWithExpressions)aggregateRoot).MappingHandlers)
            {
                if (mappinghandler.ActionMethodInfo.IsStatic)
                {
                    var message = String.Format("The method {0}.{1} could not be mapped as an event handler, since it is static.", mappinghandler.ActionMethodInfo.DeclaringType.Name, mappinghandler.ActionMethodInfo.Name);
                    throw new InvalidEventHandlerMappingException(message);
                }

                var handler = CreateHandlerForMethod(aggregateRoot, mappinghandler.ActionMethodInfo, mappinghandler.Exact);
                handlers.Add(handler);
            }

            return handlers;
        }

        /// <summary>
        /// Converts the given method into an <see cref="IDomainEventHandler"/> object.
        /// </summary>
        /// <param name="aggregateRoot">The aggregateroot from which we want to invoke the method.</param>
        /// <param name="method">The method to invoke</param>
        /// <param name="exact"><b>True</b> if we need to have an exact match, otherwise <b>False</b>.</param>
        /// <returns>An <see cref="IDomainEventHandler"/> that handles the execution of the given method.</returns>
        private static IDomainEventHandler CreateHandlerForMethod(object aggregateRoot, MethodInfo method, bool exact)
        {
            Type firstParameterType = method.GetParameters().First().ParameterType;

            Action<DomainEvent> handler = e => method.Invoke(aggregateRoot, new object[] { e });
            return new TypeThresholdedActionBasedDomainEventHandler(handler, firstParameterType, exact);
        }
    }
}