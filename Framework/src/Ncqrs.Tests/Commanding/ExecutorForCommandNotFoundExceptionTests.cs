using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using Ncqrs.Commanding.CommandExecution;
using Xunit;
using Ncqrs.Commanding;

namespace Ncqrs.Tests.Commanding
{
    
    public class ExecutorForCommandNotFoundExceptionTests
    {
        [Fact]
        public void Constructing_an_instance_should_initialize_the_message()
        {
            String message = "Hello world";
            Type aInstanceType = typeof(String);

            var target = new ExecutorForCommandNotFoundException(aInstanceType, message);

            target.Message.Should().Be(message);
        }

        [Fact]
        public void Constructing_an_instance_should_initialize_the_instance_type()
        {
            String message = "Hello world";
            Type theCommandType = typeof(ICommand);

            var target = new ExecutorForCommandNotFoundException(theCommandType, message);

            target.CommandType.Should().Be(theCommandType);
        }

        [Fact]
        public void Constructing_an_instance_should_initialize_the_inner_exception()
        {
            String aMessage = "Hello world";
            Type aCommandType = typeof(ICommand);
            var theInnerException = new Exception();

            var target = new ExecutorForCommandNotFoundException(aCommandType, aMessage, theInnerException);

            target.InnerException.Should().Be(theInnerException);
        }

        [Fact]
        public void It_should_be_serializable()
        {
            String aMessage = "Hello world";
            Type aCommandType = typeof(ICommand);

            var theException = new ExecutorForCommandNotFoundException(aCommandType, aMessage);
            ExecutorForCommandNotFoundException deserializedException = null;

            using (var buffer = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(buffer, theException);

                buffer.Seek(0, SeekOrigin.Begin);
                deserializedException = (ExecutorForCommandNotFoundException)formatter.Deserialize(buffer);
            }

            deserializedException.Should().NotBeNull();
        }
    }
}
