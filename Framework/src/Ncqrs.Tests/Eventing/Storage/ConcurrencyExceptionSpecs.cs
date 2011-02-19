using System;
using FluentAssertions;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Storage
{
    [TestFixture]
    public class ConcurrencyExceptionSpecs : BaseExceptionTests<ConcurrencyException>
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

        protected override ConcurrencyException Create()
        {
            return new ConcurrencyException(Guid.NewGuid(), 15);
        }

        protected override void VerifyDeserialized(ConcurrencyException created, ConcurrencyException deserialized)
        {
            deserialized.EventSourceId.Should().Be(created.EventSourceId);
            deserialized.EventSourceVersion.Should().Be(created.EventSourceVersion);
        }
    }
}
