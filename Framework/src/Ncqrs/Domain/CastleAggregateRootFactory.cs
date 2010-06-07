using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Ncqrs.Domain.Mapping;

namespace Ncqrs.Domain
{
    public class CastleAggregateRootFactory : IAggregateRootFactory
    {
        private readonly List<MixinConvention> _conventions = new List<MixinConvention>();

        public CastleAggregateRootFactory()
        {
            RegisterDefaultConventions();
        }

        public object CreateInstance(Type aggregateRootPocoType, params object[] constructorArguments)
        {
            var generator = new ProxyGenerator();
            var options = new ProxyGenerationOptions();
            var byConvention = new ConventionBasedDomainEventHandlerMappingStrategy();
            options.AddMixinInstance(byConvention);
            var aggregateRoot = new AggregateRoot();
            options.AddMixinInstance(aggregateRoot);
            var result = generator.CreateClassProxy(aggregateRootPocoType, options, constructorArguments);
            byConvention.Initialize(aggregateRootPocoType, result);
            return result;
        }

        public void RegisterMixinConvention(Func<Type, bool> typeSelector, Func<Type, object[], IAggregateRootMixin> mixinInstanceCostructor)
        {
            _conventions.Add(new MixinConvention(typeSelector, mixinInstanceCostructor));
        }

        private void RegisterDefaultConventions()
        {
            MapByConvention();
            MapWithAttributes();
            MapWithExpressions();
        }

        private void MapByConvention()
        {
            RegisterMixinConvention(
                x => typeof (IAggregateRootMappedByConvention).IsAssignableFrom(x),
                (type, args) => new ConventionBasedDomainEventHandlerMappingStrategy());
        }

        private void MapWithAttributes()
        {
            RegisterMixinConvention(
                x => typeof(IAggregateRootMappedWithAttributes).IsAssignableFrom(x),
                (type, args) => new AttributeBasedDomainEventHandlerMappingStrategy());
        }

        private void MapWithExpressions()
        {
            RegisterMixinConvention(
                x => typeof(IAggregateRootMappedWithExpressions).IsAssignableFrom(x),
                (type, args) => new ExpressionBasedDomainEventHandlerMappingStrategy());
        }
    }
}