using System;
using System.Reflection;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing;
using Ncqrs.Eventing.Sourcing.Mapping;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Tests.Domain
{
    [TestFixture]
    public class AggregateRootMappedByExpressionTests
    {
        public class EventForPublicMethod
        {}

        public class EventForProtectedMethod
        {}

        public class EventForPrivateMethod
        {}

        public class EventForNoEventHandlerMethod
        {}

        public class EventForMethodWithWrongMethodName
        {}

        public class TheAggregateRoot : AggregateRootMappedWithExpressions
        {
            public int OnEventForPublicMethodInvokedCount;
            public int OnEventForProtectedMethodInvokeCount;
            public int OnEventForPrivateMethodInvokeCount;
            public int OnEventForNoEventHandlerMethodInvokeCount;
            public int FooBarEventForMethodWithWrongMethodNameInvokeCount;
            
            public override void InitializeEventHandlers()
            {
                Map<EventForPublicMethod>().ToHandler(x => OnEventForPublicMethod(x));
                Map<EventForProtectedMethod>().ToHandler(x => OnEventForProtectedMethod(x));
                Map<EventForPrivateMethod>().ToHandler(x => OnEventForPrivateMethod(x));
            }

            public virtual void OnEventForPublicMethod(EventForPublicMethod e)
            {
                OnEventForPublicMethodInvokedCount++;
            }

            protected virtual void OnEventForProtectedMethod(EventForProtectedMethod e)
            {
                OnEventForProtectedMethodInvokeCount++;
            }

            private virtual void OnEventForPrivateMethod(EventForPrivateMethod e)
            {
                OnEventForPrivateMethodInvokeCount++;
            }

        }

        [Test]
        public void Initializing_one_should_set_the_mapping_strategy_to_convention_based()
        {
            var aggregateRoot = MockRepository.GenerateMock<AggregateRootMappedWithExpressions>();
            var field = aggregateRoot.GetType().BaseType.BaseType.GetField("_mappingStrategy", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

            var theStrategy = field.GetValue(aggregateRoot);
            theStrategy.Should().BeOfType<ExpressionBasedEventHandlerMappingStrategy>();
        }

        [Test]
        public void Public_event_handlers_should_be_mapped()
        {
            using (var work = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork(Guid.NewGuid()))
            {
                var target = new TheAggregateRoot();
                target.ApplyEvent(new EventForPublicMethod());
                target.OnEventForPublicMethodInvokedCount.Should().Be(1);
            }
        }

        [Test]
        public void Protected_event_handlers_should_be_mapped()
        {
            using (var work = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork(Guid.NewGuid()))
            {
                var target = new TheAggregateRoot();

                target.ApplyEvent(new EventForProtectedMethod());

                target.OnEventForProtectedMethodInvokeCount.Should().Be(1);
            }
        }

        [Test]
        public void Private_event_handlers_should_be_mapped()
        {
            using (var work = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork(Guid.NewGuid()))
            {
                var target = new TheAggregateRoot();

                target.ApplyEvent(new EventForPrivateMethod());

                target.OnEventForPrivateMethodInvokeCount.Should().Be(1);
            }
        }


        [Test]
        public void Methods_marked_as_no_event_handler_should_not_be_mapped()
        {
            using (var work = NcqrsEnvironment.Get<IUnitOfWorkFactory>().CreateUnitOfWork(Guid.NewGuid()))
            {
                var target = new TheAggregateRoot();

                Action act = () => target.ApplyEvent(new EventForNoEventHandlerMethod());

                act.ShouldThrow<EventNotHandledException>();
            }
        }
    }
}
