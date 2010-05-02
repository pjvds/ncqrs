using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Storage
{
    [TestFixture]
    public class ConcurrencyExceptionSpecs
    {
        [Test]
        public void Constructing_it_should_initialize_the_right_members()
        {
            Guid eventSourceId = Guid.NewGuid();
            long eventSourceVersion = 15;

            var ex = new ConcurrencyException(eventSourceId, eventSourceVersion);

            ex.EventSourceId.Should().Be(eventSourceId);
            ex.EventSourceVersion.Should().Be(eventSourceVersion);
        }

        [Test]
        public void It_should_be_serializable()
        {
            var theException = new ConcurrencyException(Guid.NewGuid(), 15);
            ConcurrencyException deserializedException = null;

            using (var buffer = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(buffer, theException);

                buffer.Seek(0, SeekOrigin.Begin);
                deserializedException = (ConcurrencyException)formatter.Deserialize(buffer);
            }

            deserializedException.Should().NotBeNull();
        }
    }
}
