using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ncqrs.Domain.Mapping
{
    public class ExpressionHandlerImpl<T> : IExpressionHandler<T> where T : DomainEvent
    {
        private Action<T> _mappingaction;
        private bool _matchexact;

        public MethodInfo ActionMethodInfo
        {
            get { return _mappingaction.Method; }
        }

        public IExpressionHandler<T> ToHandler(Action<T> mappingaction)
        {
            if (mappingaction != null)
                _mappingaction = mappingaction;

            return this;
        }

        public IExpressionHandler<T> MatchExact()
        {
            _matchexact = true;
            return this;
        }

        public bool Exact
        {
            get { return _matchexact; }
        }
    }
}
