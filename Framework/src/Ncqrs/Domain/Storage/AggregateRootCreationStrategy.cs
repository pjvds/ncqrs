using System;
using System.Reflection;

namespace Ncqrs.Domain.Storage
{
    public abstract class AggregateRootCreationStrategy 
        : IAggregateRootCreationStrategy
    {
        public AggregateRoot CreateAggregateRoot(Type aggregateRootType)
        {
            if (!aggregateRootType.IsSubclassOf(typeof(AggregateRoot)))
            {
                var msg = string.Format("Specified type {0} is not a subclass of AggregateRoot class.", aggregateRootType.FullName);
                throw new ArgumentOutOfRangeException("aggregateRootType", msg);
            }

            return CreateAggregateRootFromType(aggregateRootType);
        }

        protected abstract AggregateRoot CreateAggregateRootFromType(Type aggregateRootType);

        public T CreateAggregateRoot<T>() where T : AggregateRoot
        {
            return (T)CreateAggregateRoot(typeof(T));
        }
    }
}
