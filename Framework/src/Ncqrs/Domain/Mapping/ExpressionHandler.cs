using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ncqrs.Domain.Mapping
{
    public abstract class ExpressionHandler
    {
        public MethodInfo ActionMethodInfo
        { get; protected set; }

        public bool Exact
        { get; protected set; }
    }

    public sealed class ExpressionHandler<T> : ExpressionHandler where T : DomainEvent
    {
        private Action<T> _mappingaction;

        public ExpressionHandler<T> ToHandler(Action<T> mappingaction)
        {
            if (mappingaction != null)
            {
                _mappingaction = mappingaction;
                ActionMethodInfo = mappingaction.Method;
            }

            return this;
        }

        public ExpressionHandler<T> MatchExact()
        {
            Exact = true;
            return this;
        }
    }
}