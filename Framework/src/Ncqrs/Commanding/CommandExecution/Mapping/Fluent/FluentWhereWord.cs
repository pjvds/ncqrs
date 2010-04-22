using System;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    public class FluentWhereWord<T>
    {
        private readonly T _t;

        public FluentWhereWord(T t)
        {
            _t = t;
        }

        public T Where
        {
            get
            {
                return _t;
            }
        }
    }
}
