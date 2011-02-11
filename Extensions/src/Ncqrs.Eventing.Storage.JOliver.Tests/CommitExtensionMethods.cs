using System;
using System.Collections.Generic;
using EventStore;

namespace Ncqrs.Eventing.Storage.JOliver.Tests
{
    internal static class CommitExtensionMethods
    {
        public static Commit BuildAttempt(this Guid streamId, DateTime now)
        {
            var commitId = Guid.NewGuid();
            var messages = new List<EventMessage>
                               {
                                   CreateEvent(commitId, 1),
                                   CreateEvent(commitId, 2)
                               };

            return new Commit(
                streamId,
                2,
                commitId,
                1,
                now,
                new Dictionary<string, object> { { "A header", "A string value" }, { "Another header", 2 } },
                messages);
        }

        private static EventMessage CreateEvent(Guid commitId, int sequence)
        {
            return new EventMessage
                       {
                           Body = new StoredEvent
                                      {
                                          Body = new SomeDomainEvent {SomeProperty = "Test"},
                                          CommitId = commitId,
                                          EventId = Guid.NewGuid(),
                                          MajorVersion = 1,
                                          MinorVersion = 0,
                                          Sequence = sequence,
                                          TimeStamp = DateTime.Now
                                      }
                       };
        }

        public static Commit BuildAttempt(this Guid streamId)
        {
            return streamId.BuildAttempt(DateTime.UtcNow);
        }
        public static Commit BuildNextAttempt(this Commit commit)
        {
            var messages = new List<EventMessage>
                               {
                                   CreateEvent(commit.CommitId, commit.StreamRevision + 1),
                                   CreateEvent(commit.CommitId, commit.StreamRevision + 2)
                               };

            return new Commit(
                commit.StreamId,
                commit.StreamRevision + 2,
                Guid.NewGuid(),
                commit.StreamRevision,
                commit.CommitStamp,
                new Dictionary<string, object>(),
                messages);
        }

        [Serializable]
        public class SomeDomainEvent
        {
            public string SomeProperty { get; set; }
        }
    }
}