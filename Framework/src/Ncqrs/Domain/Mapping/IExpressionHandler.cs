using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ncqrs.Domain.Mapping
{
    public interface IExpressionHandler
    {
        bool Exact { get; }
        MethodInfo ActionMethodInfo { get; }
    }

    public interface IExpressionHandler<T> : IExpressionHandler where T : DomainEvent
    {
        IExpressionHandler<T> ToHandler(Action<T> act);
        IExpressionHandler<T> MatchExact();
    }
}