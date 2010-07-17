using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using Ncqrs.Config;
using NUnit.Framework;

namespace Ncqrs.Tests.Config
{
    [TestFixture]
    public class InstanceNotFoundInEnvironmentConfigurationExceptionTests
    {
        [Test]
        public void Constructing_an_instance_should_initialize_the_message()
        {
            String message = "Hello world";
            Type aInstanceType = typeof(String);

            var target = new InstanceNotFoundInEnvironmentConfigurationException(aInstanceType, message);

            target.Message.Should().Be(message);
        }

        [Test]
        public void Constructing_an_instance_should_initialize_the_instance_type()
        {
            String message = "Hello world";
            Type theInstanceType = typeof(String);

            var target = new InstanceNotFoundInEnvironmentConfigurationException(theInstanceType, message);

            target.RequestedType.Should().Be(theInstanceType);
        }

        [Test]
        public void Constructing_an_instance_should_initialize_the_inner_exception()
        {
            String aMessage = "Hello world";
            Type aInstanceType = typeof (String);
            var theInnerException = new Exception();

            var target = new InstanceNotFoundInEnvironmentConfigurationException(aInstanceType, aMessage, theInnerException);

            target.InnerException.Should().Be(theInnerException);
        }

        [Test]
        public void It_should_be_serializable()
        {
            var aMessage = "Hello world";
            Type aInstanceType = typeof(String);

            var theException = new InstanceNotFoundInEnvironmentConfigurationException(aInstanceType, aMessage);
            InstanceNotFoundInEnvironmentConfigurationException deserializedException = null;

            using (var buffer = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(buffer, theException);

                buffer.Seek(0, SeekOrigin.Begin);
                deserializedException = (InstanceNotFoundInEnvironmentConfigurationException)formatter.Deserialize(buffer);
            }

            deserializedException.Should().NotBeNull();
        }
    }
}
