using System;

namespace Ncqrs.Domain
{
    public class MixinConvention
    {
        private readonly Func<Type, bool> _typeSelector;
        private readonly Func<Type, object[], IAggregateRootMixin> _mixinInstanceCostructor;

        public MixinConvention(Func<Type, bool> typeSelector, Func<Type, object[], IAggregateRootMixin> mixinInstanceCostructor)
        {
            _typeSelector = typeSelector;
            _mixinInstanceCostructor = mixinInstanceCostructor;
        }

        public bool ShouldMix(Type aggregateRootPocoType)
        {
            return _typeSelector(aggregateRootPocoType);
        }

        public IAggregateRootMixin CreateMixin(Type aggregateRootPocoType, object[] constructorArguments)
        {
            return _mixinInstanceCostructor(aggregateRootPocoType, constructorArguments);
        }
    }
}