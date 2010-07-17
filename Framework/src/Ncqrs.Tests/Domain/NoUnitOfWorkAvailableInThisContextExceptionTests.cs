using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using Ncqrs.Domain;
using NUnit.Framework;

namespace Ncqrs.Tests.Domain
{
    [TestFixture]
    public class NoUnitOfWorkAvailableInThisContextExceptionTests
    {
        [Test]
        public void Constructing_an_instance_should_initialize_the_message()
        {
            String message = "Hello world";

            var target = new NoUnitOfWorkAvailableInThisContextException(message);

            target.Message.Should().Be(message);
        }

        [Test]
        public void Constructing_an_instance_should_initialize_the_inner_exception()
        {
            String aMessage = "Hello world";
            var theInnerException = new Exception();

            var target = new NoUnitOfWorkAvailableInThisContextException(aMessage, theInnerException);

            target.InnerException.Should().Be(theInnerException);
        }

        [Test]
        public void It_should_be_serializable()
        {
            var theException = new NoUnitOfWorkAvailableInThisContextException();
            NoUnitOfWorkAvailableInThisContextException deserializedException = null;

            using (var buffer = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(buffer, theException);

                buffer.Seek(0, SeekOrigin.Begin);
                deserializedException = (NoUnitOfWorkAvailableInThisContextException)formatter.Deserialize(buffer);
            }

            deserializedException.Should().NotBeNull();
        }
    }
}
