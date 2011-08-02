using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using NUnit.Framework;
using FluentAssertions;
using Rhino.Mocks;

namespace Ncqrs.Eventing.Storage.JOliver.Tests
{
    [TestFixture]
    public class JoesBrowsableEventStoreTests
    {
        [Test]
        public void When_fetching_for_the_first_time_and_pipeline_has_no_state_events_are_fetched_from_the_beginning()
        {
            var engine = MockRepository.GenerateMock<IPersistStreamsWithAbsoluteOrdering>();
            engine.Expect(x => x.GetLastProcessedSequentialNumber("Pipeline")).Return(0);
            engine.Expect(x => x.Fetch(0,0)).IgnoreArguments().Return(new Commit[] {});
            var sut = new JoesBrowsableEventStore(engine);
            sut.Fetch("Pipeline", 10).ToList();

            engine.VerifyAllExpectations();
        }

        [Test]
        public void When_fetching_for_the_first_time_first_returned_commit_is_returned()
        {
            var engine = MockRepository.GenerateMock<IPersistStreamsWithAbsoluteOrdering>();
            engine.Expect(x => x.GetLastProcessedSequentialNumber("Pipeline")).Return(0);
            var streamId = Guid.NewGuid();
            engine.Expect(x => x.Fetch(0,0)).IgnoreArguments().Return(
                new[]
                    {
                        CreateCommit(streamId, 1),
                        CreateCommit(streamId, 2)
                    });
            var sut = new JoesBrowsableEventStore(engine);
            var results = sut.Fetch("Pipeline", 10).ToList();

            engine.VerifyAllExpectations();
            results.Count.Should().Be(2);
        }

        [Test]
        public void When_fetching_subsequent_time_first_returned_commit_is_skipped()
        {
            var engine = MockRepository.GenerateMock<IPersistStreamsWithAbsoluteOrdering>();
            engine.Expect(x => x.GetLastProcessedSequentialNumber("Pipeline")).Return(0);
            var streamId = Guid.NewGuid();
            engine.Expect(x => x.Fetch(0,10)).Return(
                new[]
                    {
                        CreateCommit(streamId, 1),
                        CreateCommit(streamId, 2)
                    });
            engine.Expect(x => x.Fetch(2,10)).Return(
                new[]
                    {
                        CreateCommit(streamId, 3),
                        CreateCommit(streamId, 4)
                    });
            var sut = new JoesBrowsableEventStore(engine);
            sut.Fetch("Pipeline", 10).ToList();

            var results = sut.Fetch("Pipeline", 10).ToList();

            engine.VerifyAllExpectations();
            results.Count.Should().Be(2);
        }

        private static Commit CreateCommit(Guid streamId, long sequentialId)
        {
            return new Commit(streamId, 1, Guid.NewGuid(), 1, DateTime.Now,
                              new Dictionary<string, object>()
                                  {
                                      {"SequentialId", sequentialId}
                                  },
                              new List<EventMessage>()
                                  {
                                      new EventMessage()
                                          {
                                              Body = new StoredEvent()
                                          }
                                  });
        }
    }
}