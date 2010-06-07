using System;

namespace Ncqrs.Domain
{
    public interface IAggregateRootFactory
    {
        object CreateInstance(Type aggregateRootPocoType, params object[] constructorArguments);
        void RegisterMixinConvention(Func<Type, bool> typeSelector, Func<Type, object[], IAggregateRootMixin> mixinInstanceCostructor);
    }    
}