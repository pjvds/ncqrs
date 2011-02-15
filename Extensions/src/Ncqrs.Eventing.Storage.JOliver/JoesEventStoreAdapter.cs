using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Persistence;

namespace Ncqrs.Eventing.Storage.JOliver
{
    public class JoesEventStoreAdapter : IEventStore
    {
        private readonly ICommitEvents _wrappedEventStore;

        public JoesEventStoreAdapter(IPersistStreams streamPersister)
        {
            if (!(streamPersister is IPersistStreamsWithAbsoluteOrdering))
            {
                throw new ArgumentException("The stream store must impement IPersistStreamsWithAbsoluteOrdering in order to be used with JoesEventStoreAdapter", "streamStore");
            }
            _wrappedEventStore = new OptimisticEventStore(streamPersister, new NullDispatcher());
        }
        
        

        public CommittedEventStream ReadFrom(Guid id, long minVersion, long maxVersion)
        {
            int maxRevision = maxVersion == long.MaxValue ? int.MaxValue : (int) maxVersion;
            int minRevision = minVersion == long.MinValue ? int.MinValue : (int) minVersion;

            var committedEvents = _wrappedEventStore.GetFrom(id, minRevision, maxRevision)
                .SelectMany(x => x.Events)
                .Select(x => x.Body)
                .Cast<StoredEvent>()
                .Select(x => x.Convert(id));
            return new CommittedEventStream(committedEvents);
        }

        public void Store(UncommittedEventStream eventStream)
        {
            if (!eventStream.HasSingleSource)
            {
                throw new NotSupportedException("This event store don't support events streams with multiple sources.");
            }
            
            var sourceInformation = eventStream.Sources.Single();

            var events = eventStream.Select(x => new EventMessage
                                                     {
                                                         Body = new StoredEvent
                                                                    {
                                                                        Body = x.Payload,
                                                                        CommitId = eventStream.CommitId,
                                                                        EventId = x.EventIdentifier,
                                                                        MajorVersion = x.EventVersion.Major,
                                                                        MinorVersion = x.EventVersion.Minor,
                                                                        Sequence = x.EventSequence,
                                                                        TimeStamp = x.EventTimeStamp
                                                                    }
                                                     }).ToList();

            var commitAttempt = new Commit(eventStream.SourceId, (int)sourceInformation.CurrentVersion,
                                    eventStream.CommitId,
                                    GetInitialVersion(sourceInformation),
                                    DateTime.UtcNow,
                                    new Dictionary<string, object>(),
                                    events);
            
            _wrappedEventStore.Commit(commitAttempt);
        }

        private static int GetInitialVersion(EventSourceInformation sourceInformation)
        {
            //JO Event Store assumes, that version number starts from 1.
            var value = (int)sourceInformation.InitialVersion;
            return value == 0 ? 1 : value; 
        }

        private class NullDispatcher : IDispatchCommits
        {
            public void Dispose()
            {                
            }

            public void Dispatch(Commit commit)
            {             
            }
        }
    }
}
