using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using EventStore.Serialization;
using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;
using NUnit.Framework;
using FluentAssertions;

namespace Ncqrs.Eventing.Storage.JOliver.Tests
{
    [TestFixture]
    public class AbsoluteOrderingSqlPersistenceEngineTests
    {
        private IPersistStreamsWithAbsoluteOrdering _sut;

        [Test]
        public void Pipeline_state_is_persisted()
        {
            string pipelineName = Guid.NewGuid().ToString();
            var emptyState = _sut.GetLastProcessedSequentialNumber(pipelineName);

            emptyState.Should().Be(0);
            var headers = new Dictionary<string, object>();
            var commit = new Commit(Guid.NewGuid(), 1, Guid.NewGuid(), 1, DateTime.UtcNow, headers, new List<EventMessage>(){new EventMessage()});
            _sut.Commit(commit);
            _sut.MarkLastProcessed(pipelineName, commit.StreamId, commit.CommitId);
            var setState = _sut.GetLastProcessedSequentialNumber(pipelineName);
            setState.Should().NotBe(0);
        }

        [Test]
        public void Fetching_from_given_id_should_return_commit_with_given_id_and_newer()
        {
            var streamId = Guid.NewGuid();
            var firstCommit = streamId.BuildAttempt();
            var secondCommit = firstCommit.BuildNextAttempt();
            var thirdCommit = secondCommit.BuildNextAttempt();

            _sut.Commit(firstCommit);
            _sut.Commit(secondCommit);
            _sut.Commit(thirdCommit);

            var all = _sut.Fetch(0,100).ToList();
            var second = all[all.Count-2];
            second.CommitId.Should().Be(secondCommit.CommitId);

            var secondAndLater = _sut.Fetch((long)second.Headers["SequentialId"],100).ToList();
            secondAndLater.Count.Should().Be(2);
            second = secondAndLater[0];
            second.CommitId.Should().Be(secondCommit.CommitId);
        }

        [SetUp]
        public void Initialize()
        {
            var factory = new AbsoluteOrderingSqlPersistenceFactory("EventStore", new BinarySerializer(), false);
            _sut = (IPersistStreamsWithAbsoluteOrdering)factory.Build();
            _sut.Initialize();
        }
    }

}