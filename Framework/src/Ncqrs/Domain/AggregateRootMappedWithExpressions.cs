using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Mapping;
using System.Linq.Expressions;
using System.Reflection;

namespace Ncqrs.Domain
{
    public abstract class AggregateRootMappedWithExpressions : MappedAggregateRoot<AggregateRootMappedWithExpressions>
    {
        private IList<IExpressionHandler> _mappinghandlers = new List<IExpressionHandler>();

        internal IEnumerable<IExpressionHandler> MappingHandlers
        {
            get { return _mappinghandlers; }
        }

        protected AggregateRootMappedWithExpressions() 
            : base(new ExpressionBasedDomainEventHandlerMappingStrategy())
        {
            InitMappingRules();
        }
        
        protected IExpressionHandler<T> Map<T>() where T : DomainEvent
        {
            var handler = new ExpressionHandlerImpl<T>();
            _mappinghandlers.Add(handler);

            return handler;
        }

        public abstract void InitMappingRules();
    }
}