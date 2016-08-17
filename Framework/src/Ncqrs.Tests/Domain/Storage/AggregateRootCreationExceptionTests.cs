using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using Ncqrs.Domain.Storage;
using Xunit;

namespace Ncqrs.Tests.Domain.Storage
{
    
    public class AggregateRootCreationExceptionTests
    {
        [Fact]
        public void Constructing_an_instance_should_initialize_the_message()
        {
            String message = "Hello world";

            var target = new AggregateRootCreationException(message);

            target.Message.Should().Be(message);
        }

        [Fact]
        public void Constructing_an_instance_should_initialize_the_inner_exception()
        {
            String aMessage = "Hello world";
            var theInnerException = new Exception();

            var target = new AggregateRootCreationException(aMessage, theInnerException);

            target.InnerException.Should().Be(theInnerException);
        }

        [Fact]
        public void It_should_be_serializable()
        {
            var aMessage = "Hello world";

            var theException = new AggregateRootCreationException(aMessage);
            AggregateRootCreationException deserializedException = null;

            using (var buffer = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(buffer, theException);

                buffer.Seek(0, SeekOrigin.Begin);
                deserializedException = (AggregateRootCreationException)formatter.Deserialize(buffer);
            }

            deserializedException.Should().NotBeNull();
        }
    }
}
