using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Ncqrs.Domain.Mapping
{
    /// <summary>
    /// A internal event handler mapping strategy that maps methods as an event handler based upon an expression statement.
    /// <remarks>
    /// </summary>
    public class ExpressionBasedDomainEventHandlerMappingStrategy : IDomainEventHandlerMappingStrategy<AggregateRootMappedWithExpressions>
    {
        private static IDomainEventHandler CreateHandlerForMethod(AggregateRoot aggregateRoot, MethodInfo method, bool exact)
        {
            Type firstParameterType = method.GetParameters().First().ParameterType;
            
            Action<DomainEvent> handler = e => method.Invoke(aggregateRoot, new object[] { e });
            return new TypeThresholdedActionBasedDomainEventHandler(handler, firstParameterType, exact);
        }

        public IEnumerable<IDomainEventHandler> GetEventHandlersFromAggregateRoot(AggregateRootMappedWithExpressions aggregateRoot)
        {
            Contract.Requires<ArgumentNullException>(aggregateRoot != null, "The aggregateRoot cannot be null.");
            
            var handlers = new List<IDomainEventHandler>();

            foreach (IExpressionHandler mappinghandler in aggregateRoot.MappingHandlers)
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
    }
}