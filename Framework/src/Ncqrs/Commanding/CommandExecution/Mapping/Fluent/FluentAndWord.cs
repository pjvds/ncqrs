using System;

namespace Ncqrs.Commanding.CommandExecution.Mapping.Fluent
{
    public class FluentAndWord<T>
    {
        private readonly T _t;

        public FluentAndWord(T t)
        {
            _t = t;
        }

        public T And
        {
            get
            {
                return _t;
            }
        }
    }
}
