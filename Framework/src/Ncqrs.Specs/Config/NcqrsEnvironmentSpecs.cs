using System;
using FluentAssertions;
using Ncqrs.Config;
using Ncqrs.Domain;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Specs.Config
{
    [TestFixture]
    public class NcqrsEnvironmentSpecs
    {
        public interface IFoo
        {}

        public class Foo : IFoo
        {}

        [Test]
        public void When_get_is_called_when_the_environmemt_is_not_configured_it_should_throw_an_exception()
        {
            Assert.Ignore();
            //Action act = () => NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            //act.ShouldThrow<>();
        }

        [Test]
        public void When_get_is_called_the_call_should_be_redirected_to_the_configuration()
        {
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
            // Arrange
            var repository = new MockRepository();
            NcqrsEnvironment.Configure(repository.StrictMock<IEnvironmentConfiguration>());

            // Act
            Action act = () => NcqrsEnvironment.Get<IUnitOfWorkFactory>();

            // Assert
            act.ShouldThrow<InstanceNotFoundInEnvironmentConfigurationException>();
        }
    }
}
