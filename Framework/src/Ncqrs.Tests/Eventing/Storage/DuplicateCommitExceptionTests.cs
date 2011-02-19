using System;
using FluentAssertions;
using Ncqrs.Eventing.Storage;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Storage
{
    [TestFixture]
    public class DuplicateCommitExceptionTests : BaseExceptionTests<DuplicateCommitException>
    {
        protected override DuplicateCommitException Create()
        {
            return new DuplicateCommitException(Guid.NewGuid(), Guid.NewGuid());
        }

        protected override void VerifyDeserialized(DuplicateCommitException created, DuplicateCommitException deserialized)
        {
            deserialized.EventSourceId.Should().Be(created.EventSourceId);
            deserialized.CommitId.Should().Be(created.CommitId);
        }
    }
}