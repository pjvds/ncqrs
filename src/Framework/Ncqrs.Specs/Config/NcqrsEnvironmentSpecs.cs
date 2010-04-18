using System;
using FluentAssertions;
using Ncqrs.Config;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ncqrs.Specs.Config
{
    [TestFixture]
    public class NcqrsEnvironmentSpecs
    {
        [Test]
        public void When_get_is_called_when_the_environmemt_is_not_configured_it_should_throw_an_exception()
        {
            Action act = () => NcqrsEnvironment.Get<NcqrsEnvironment>();
            act.ShouldThrow<EnvironmentNotConfiguredException>();
        }

        [Test]
        public void When_get_is_called_the_call_should_be_redirected_to_the_configuration()
        {
            Assert.Ignore();

            var repository = new MockRepository();

            // Arrange
            var mock = repository.StrictMock<IEnvironmentConfiguration>();
            String outParameter;
            String outResultValue = "result";
            mock.Expect(x => x.TryGet(out outParameter)).Return(true).OutRef(outResultValue);

            NcqrsEnvironment.Configure(mock);

            // Act
            NcqrsEnvironment.Get<String>();

            // Assert
            mock.VerifyAllExpectations();
        }

        [Test]
        public void When_get_is_called_but_the_source_did_not_return_an_intance_an_exception_should_be_thrown()
        {
            // Arrange
            var repository = new MockRepository();
            NcqrsEnvironment.Configure(repository.StrictMock<IEnvironmentConfiguration>());

            // Act
            Action act = () => NcqrsEnvironment.Get<NcqrsEnvironment>();

            // Assert
            act.ShouldThrow<InstanceNotFoundInEnvironmentConfigurationException>();
        }
    }
}
