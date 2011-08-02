//using System;
//using System.Collections.Generic;
//using System.Linq;
//using EventStore;
//using EventStore.Persistence;
//using EventStore.Serialization;
//using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;
//using NUnit.Framework;
//using FluentAssertions;

//namespace Ncqrs.Eventing.Storage.JOliver.Tests
//{
//    [TestFixture]
//    public class JoesEventStoreAdapterTests
//    {
//        private IPersistStreamsWithAbsoluteOrdering _persistenceEngine;
//        private JoesSnapshotStoreAdapter _sut;
//        private Guid _streamId;
//        private Guid _firstCommitId;

//        [Test]
//        public void When_reading_all_events_all_events_should_be_returned()
//        {
//            var firstCommit = _streamId.BuildAttempt();
//            var secondCommit = firstCommit.BuildNextAttempt();
//            var thirdCommit = secondCommit.BuildNextAttempt();

//            _persistenceEngine.Commit(firstCommit);
//            _persistenceEngine.Commit(secondCommit);
//            _persistenceEngine.Commit(thirdCommit);

//            var stream = _sut.ReadFrom(_streamId, long.MinValue, long.MaxValue);

//            stream.Should().HaveCount(6);
//        }

//        [Test]
//        public void When_reading_events_up_to_specific_version_only_matching_events_should_be_returned()
//        {
//            var firstCommit = _streamId.BuildAttempt();
//            var secondCommit = firstCommit.BuildNextAttempt();
//            var thirdCommit = secondCommit.BuildNextAttempt();

//            _persistenceEngine.Commit(firstCommit);
//            _persistenceEngine.Commit(secondCommit);
//            _persistenceEngine.Commit(thirdCommit);

//            var stream = _sut.ReadFrom(_streamId, long.MinValue, 3);

//            stream.Should().HaveCount(4);
//            stream.Last().EventSequence.Should().Be(4);
//        }

//        [Test]
//        public void When_reading_events_from_specific_version_only_matching_events_should_be_returned()
//        {
//            var firstCommit = _streamId.BuildAttempt();
//            var secondCommit = firstCommit.BuildNextAttempt();
//            var thirdCommit = secondCommit.BuildNextAttempt();

//            _persistenceEngine.Commit(firstCommit);
//            _persistenceEngine.Commit(secondCommit);
//            _persistenceEngine.Commit(thirdCommit);

//            var stream = _sut.ReadFrom(_streamId, 3, long.MaxValue);

//            stream.Should().HaveCount(4);
//            stream.First().EventSequence.Should().Be(3);
//        }

//        [Test]
//        public void When_persisting_events_sequence_information_should_be_stored()
//        {
//            var streamToCommit = new UncommittedEventStream(Guid.NewGuid());
//            streamToCommit.Append(new UncommittedEvent(Guid.NewGuid(), _streamId, 1, 1, DateTime.UtcNow, new object(), new Version(1,0)));
//            streamToCommit.Append(new UncommittedEvent(Guid.NewGuid(), _streamId, 2, 1, DateTime.UtcNow, new object(), new Version(1, 0)));
//            streamToCommit.Append(new UncommittedEvent(Guid.NewGuid(), _streamId, 3, 1, DateTime.UtcNow, new object(), new Version(1, 0)));

//            _sut.Store(streamToCommit);

//            var stream = _sut.ReadFrom(_streamId, long.MinValue, long.MaxValue);
//            stream.CurrentSourceVersion.Should().Be(3);
//            var events = stream.ToList();
//            for (int i = 0; i < 2; i++)
//            {
//                events[i + 1].EventSequence.Should().Be(events[i].EventSequence + 1);
//            }
//            var commits = _persistenceEngine.GetFrom(_streamId, int.MinValue, int.MaxValue).ToList();
//            commits[0].CommitSequence.Should().Be(3);
//            commits[0].StreamRevision.Should().Be(3);
//        }


//        [SetUp]
//        public void Initialize()
//        {
//            var factory = new AbsoluteOrderingSqlPersistenceFactory("EventStore", new BinarySerializer());
//            _persistenceEngine = (IPersistStreamsWithAbsoluteOrdering) factory.Build();
//            _persistenceEngine.Initialize();
//            _sut = new JoesSnapshotStoreAdapter(_persistenceEngine);
//            _streamId = Guid.NewGuid();
            
//        }
//    }
//}