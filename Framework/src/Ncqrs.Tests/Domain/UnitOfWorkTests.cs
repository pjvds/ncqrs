using System;
using FluentAssertions;
using Ncqrs.Domain;
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

        public class MyAggregateRoot : AggregateRootMappedByConvention
        {
            public String FooString
            {
                get; private set;
            }

            public MyAggregateRoot()
            {
                var e = new NewMyAggregateRootCreatedEvent();
                ApplyEvent(e);
            }

            public void Foo(string value)
            {
                var e = new FooEvent(value);
                ApplyEvent(e);
            }

            private void OnNewMyAggregateRootCreatedEvent(NewMyAggregateRootCreatedEvent e)
            {
                this.Id = e.AggregateRootId;
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
                var agg1 = new MyAggregateRoot();
                var agg2 = new MyAggregateRoot();

                agg1.Foo("a string");
                agg1.Foo("a string");
                agg2.Foo("a string");
                agg2.Foo("a string");
                agg2.Foo("a string");

                work.Accept();

                repository.AssertWasCalled(r => r.Save(agg1));
                repository.AssertWasCalled(r => r.Save(agg2));
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
                var agg1 = new MyAggregateRoot();
                var agg2 = new MyAggregateRoot();

                agg1.Foo("a string");
                agg1.Foo("a string");
                agg2.Foo("a string");
                agg2.Foo("a string");
                agg2.Foo("a string");
            }

            repository.AssertWasNotCalled(r => r.Save(null), options=>options.IgnoreArguments());
        }

        [Test]
        public void The_repository_should_be_that_same_as_set_in_the_environment()
        {
            var theRepository = MockRepository.GenerateMock<IDomainRepository>();
            NcqrsEnvironment.SetDefault<IDomainRepository>(theRepository);

            var factory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var work = factory.CreateUnitOfWork())
            {
                work.Repository.Should().Be(theRepository);
            }
        }
    }
}
