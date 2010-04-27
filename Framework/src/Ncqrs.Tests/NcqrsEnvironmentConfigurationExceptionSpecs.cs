using System;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using NUnit.Framework;
using System.IO;

namespace Ncqrs.Tests
{
    [TestFixture]
    public class NcqrsEnvironmentConfigurationExceptionSpecs
    {
        [Test]
        public void Constructing_an_instance_should_initialize_the_message()
        {
            String message = "Hello world";

            var target = new NcqrsEnvironmentException(message);

            target.Message.Should().Be(message);
        }

        [Test]
        public void Constructing_an_instance_should_initialize_the_inner_exception()
        {
            String aMessage = "Hello world";
            var theInnerException = new Exception();

            var target = new NcqrsEnvironmentException(aMessage, theInnerException);

            target.InnerException.Should().Be(theInnerException);
        }

        [Test]
        public void It_should_be_serializable()
        {
            var aMessage = "Hello world";
            var theException = new NcqrsEnvironmentException(aMessage);
            NcqrsEnvironmentException deserializedException = null;

            using (var buffer = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(buffer, theException);

                buffer.Seek(0, SeekOrigin.Begin);
                deserializedException = (NcqrsEnvironmentException)formatter.Deserialize(buffer);
            }

            deserializedException.Should().NotBeNull();
        }
    }
}
