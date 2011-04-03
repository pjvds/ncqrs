using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Domain.Storage;
using Ncqrs.Spec;
using Ninject;

namespace Ncqrs.Config.Ninject.Tests
{

    [Specification]
    public class NinjectAggregateRootCreationStrategyTests
        : BaseTestFixture
    {
        private IAggregateRootCreationStrategy _creationStrategy;
        private FakeAggregate _aggregate;

        private class FakeAggregate : AggregateRoot
        {

            public bool CorrectConstructorCalled { get; private set; }

            [Inject]
            private FakeAggregate(IClock clock)
            {
                CorrectConstructorCalled = true;
            }

            public FakeAggregate(IClock clock, Guid id)
            {
            }

        }

        protected override void Given()
        {
            base.Given();
            var settings = new NinjectSettings {InjectNonPublic = true};

            IKernel kernel = new StandardKernel(settings);
            kernel.Bind<IClock>().To<DateTimeBasedClock>();
            _creationStrategy = new NinjectAggregateRootCreationStrategy(kernel);
        }

        protected override void When()
        {
                _aggregate = _creationStrategy.CreateAggregateRoot<FakeAggregate>();
        }

        [Then]
        public void it_doesnt_throw()
        {
            CaughtException.Should().BeNull();
        }

        [Then]
        public void it_creates_the_aggregate()
        {
            _aggregate.Should().NotBeNull();
        }

        [Then]
        public void it_calls_the_right_constructor()
        {
            _aggregate.CorrectConstructorCalled.Should().BeTrue();
        }



    }

}
