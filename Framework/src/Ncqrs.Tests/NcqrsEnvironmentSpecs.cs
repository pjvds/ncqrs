using System;
using FluentAssertions;
using Ncqrs.Config;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Tests
{
    [TestFixture]
    public class NcqrsEnvironmentSpecs
    {
        public interface IFoo
        {}

        public class Foo : IFoo
        {}

        public interface IBar
        {}

        [TearDown]
        public void TearDown()
        {
            NcqrsEnvironment.Deconfigure();            
        }

        [Test]
        public void When_get_is_called_when_the_environmemt_is_not_configured_defaults_should_still_be_returned()
        {
            NcqrsEnvironment.Deconfigure();

            var defaultClock = new DateTimeBasedClock();
            NcqrsEnvironment.SetDefault<IClock>(defaultClock);

            NcqrsEnvironment.Get<IClock>().Should().Be(defaultClock);
        }

        [Test] 
        public void Configured_instance_should_over_rule_default()
        {
            var defaultClock = new DateTimeBasedClock();
            var configuredClock = MockRepository.GenerateMock<IClock>();
            IClock ingore;

            var configuration = MockRepository.GenerateMock<IEnvironmentConfiguration>();
            configuration.Stub((m) => m.TryGet(out ingore)).IgnoreArguments().OutRef(configuredClock).Return(true);

            NcqrsEnvironment.SetDefault<IClock>(defaultClock);
            NcqrsEnvironment.Configure(configuration);

            var result = NcqrsEnvironment.Get<IClock>();

            Assert.AreSame(configuredClock, result);
            Assert.AreNotSame(defaultClock, result);

            NcqrsEnvironment.Deconfigure();
        }

        [Test] 
        public void Removing_a_default_while_there_is_no_default_registered_should_not_throw_an_exception()
        {
            NcqrsEnvironment.RemoveDefault<IFoo>();
            NcqrsEnvironment.RemoveDefault<IFoo>();
        }

        [Test] 
        public void Setting_a_default_should_multiple_times_should_not_throw_an_exception()
        {
            var defaultFoo = MockRepository.GenerateMock<IFoo>();
            var newDefaultFoo = MockRepository.GenerateMock<IFoo>();

            NcqrsEnvironment.SetDefault<IFoo>(defaultFoo);
            NcqrsEnvironment.SetDefault<IFoo>(newDefaultFoo);
            NcqrsEnvironment.SetDefault<IFoo>(defaultFoo);
            NcqrsEnvironment.SetDefault<IFoo>(newDefaultFoo);
        }

        [Test]
        public void Setting_a_default_should_override_the_exiting_default()
        {
            var defaultFoo = MockRepository.GenerateMock<IFoo>();
            var newDefaultFoo = MockRepository.GenerateMock<IFoo>();

            NcqrsEnvironment.SetDefault<IFoo>(defaultFoo);
            NcqrsEnvironment.SetDefault<IFoo>(newDefaultFoo);

            var result = NcqrsEnvironment.Get<IFoo>();

            result.Should().BeSameAs(newDefaultFoo);
        }

        [Test]
        public void When_get_is_called_the_call_should_be_redirected_to_the_configuration()
        {
            NcqrsEnvironment.Deconfigure();

            // Arrange
            IFoo outParameter;
            var configuration = MockRepository.GenerateStub<IEnvironmentConfiguration>();
            configuration.Stub(x => x.TryGet(out outParameter)).Return(true).OutRef(new Foo());
            NcqrsEnvironment.Configure(configuration);

            // Act
            NcqrsEnvironment.Get<IFoo>();

            // Assert
            configuration.AssertWasCalled(x=>x.TryGet(out outParameter));
        }

        [Test]
        public void When_get_is_called_the_call_should_return_what_the_environment_configuration_returned()
        {
            NcqrsEnvironment.Deconfigure();

            // Arrange
            IFoo outParameter;
            IFoo returnValue = new Foo();

            var configuration = MockRepository.GenerateStub<IEnvironmentConfiguration>();
            configuration.Stub(x => x.TryGet(out outParameter)).Return(true).OutRef(returnValue);
            NcqrsEnvironment.Configure(configuration);

            // Act
            var result = NcqrsEnvironment.Get<IFoo>();

            // Assert
            result.Should().Be(returnValue);
        }

        [Test]
        public void When_get_is_called_but_the_source_did_not_return_an_intance_an_exception_should_be_thrown()
        {
            NcqrsEnvironment.Deconfigure();

            // Arrange
            var repository = new MockRepository();
            NcqrsEnvironment.Configure(repository.StrictMock<IEnvironmentConfiguration>());

            // Act
            Action act = () => NcqrsEnvironment.Get<IBar>();

            // Assert
            act.ShouldThrow<InstanceNotFoundInEnvironmentConfigurationException>();
        }
    }
}
