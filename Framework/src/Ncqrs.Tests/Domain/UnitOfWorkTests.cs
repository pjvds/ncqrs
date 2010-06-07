using System;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Domain.Mapping;
using Ncqrs.Eventing;
using Rhino.Mocks;
using Ncqrs.Domain.Storage;
using NUnit.Framework;

namespace Ncqrs.Tests.Domain
{
    public class UnitOfWorkTests
    {
        public class NewMyAggregateRootCreatedEvent : DomainEvent
        {
        }

        public class FooEvent : DomainEvent
        {
            public FooEvent(string value)
            {
                FooStringValue = value;
            }

            public string FooStringValue { get; private set; }
        }

        public class MyAggregateRoot : IAggregateRoot, IAggregateRootMappedByConvention
        {
            public String FooString
            {
                get; private set;
            }

            public MyAggregateRoot()
            {
                var e = new NewMyAggregateRootCreatedEvent();
                this.ApplyEvent(e);
            }

            public void Foo(string value)
            {
                var e = new FooEvent(value);
                this.ApplyEvent(e);
            }

            private void OnNewMyAggregateRootCreatedEvent(NewMyAggregateRootCreatedEvent e)
            {
                this.SetId(e.AggregateRootId);
            }

            private void OnFoo(FooEvent e)
            {
                FooString = e.FooStringValue;
            }
        }

        [Test]
        public void Accepting_it_should_cause_a_save_for_all_dirty_instances()
        {
            var repository = MockRepository.GenerateMock<IDomainRepository>();
            NcqrsEnvironment.SetDefault<IDomainRepository>(repository);

            var factory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using(var work = factory.CreateUnitOfWork())
            {
                var agg1 = work.Create<MyAggregateRoot>();
                var agg2 = work.Create<MyAggregateRoot>();

                agg1.Foo("a string");
                agg1.Foo("a string");
                agg2.Foo("a string");
                agg2.Foo("a string");
                agg2.Foo("a string");

                work.Accept();

                repository.AssertWasCalled(r => r.Save((IEventSource)agg1));
                repository.AssertWasCalled(r => r.Save((IEventSource)agg2));
            }
        }

        [Test]
        public void Never_accepting_it_should_not_call_save_for_the_dirty_instances()
        {
            var repository = MockRepository.GenerateMock<IDomainRepository>();
            NcqrsEnvironment.SetDefault<IDomainRepository>(repository);

            var factory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = factory.CreateUnitOfWork())
            {
                var agg1 = work.Create<MyAggregateRoot>();
                var agg2 = work.Create<MyAggregateRoot>();

                agg1.Foo("a string");
                agg1.Foo("a string");
                agg2.Foo("a string");
                agg2.Foo("a string");
                agg2.Foo("a string");
            }

            repository.AssertWasNotCalled(r => r.Save(null), options=>options.IgnoreArguments());
        }

        [Test]
        public void Getting_an_aggregate_root_shold_redirect_call_to_repository()
        {
            var theRepository = MockRepository.GenerateMock<IDomainRepository>();
            NcqrsEnvironment.SetDefault<IDomainRepository>(theRepository);

            var factory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = factory.CreateUnitOfWork())
            {
                var theAggregate = work.Create<MyAggregateRoot>();
                var aId = Guid.NewGuid();

                theRepository.Expect(r => r.GetById<MyAggregateRoot>(aId)).Return(theAggregate);

                work.GetById<MyAggregateRoot>(aId).Should().Be(theAggregate);
            }

            theRepository.VerifyAllExpectations();
        }
    }
}
