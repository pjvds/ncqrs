using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Storage
{
    public abstract class BaseExceptionTests<T> where T : Exception
    {
        protected abstract T Create();
        protected abstract void VerifyDeserialized(T created, T deserialized);

        [Test]
        public void It_should_be_serializable()
        {
            var theException = Create();
            T deserializedException = null;

            using (var buffer = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(buffer, theException);

                buffer.Seek(0, SeekOrigin.Begin);
                deserializedException = (T)formatter.Deserialize(buffer);
            }

            deserializedException.Should().NotBeNull();
            VerifyDeserialized(theException, deserializedException);
        }
    }
}