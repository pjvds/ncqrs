using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain.Mapping;
using System.Linq.Expressions;

namespace Ncqrs.Domain
{
    public abstract class AggregateRootMappedWithExpressions : MappedAggregateRoot<AggregateRootMappedWithExpressions>
    {
        protected AggregateRootMappedWithExpressions() 
            : base(new ExpressionBasedDomainEventHandlerMappingStrategy())
        {}

        public abstract void InitMappingRules();
    }
}
