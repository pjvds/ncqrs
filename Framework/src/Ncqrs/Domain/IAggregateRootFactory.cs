using System;

namespace Ncqrs.Domain
{
    public interface IAggregateRootFactory
    {
        object CreateInstance(Type aggregateRootPocoType, params object[] constructorArguments);
        void RegisterMixin(Func<Type, bool> typeSelector, IAggregateRootMixin mixinInstance);
    }

    public class CastleAggregateRootFactory : IAggregateRootFactory
    {
        public object CreateInstance(Type aggregateRootPocoType, params object[] constructorArguments)
        {
            throw new NotImplementedException();
        }

        public void RegisterMixin(Func<Type, bool> typeSelector, IAggregateRootMixin mixinInstance)
        {
            throw new NotImplementedException();
        }
    }
}