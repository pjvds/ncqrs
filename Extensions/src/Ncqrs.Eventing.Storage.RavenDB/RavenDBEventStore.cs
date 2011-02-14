﻿using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Document;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.RavenDB
{
    public class RavenDBEventStore : IEventStore
    {
        private readonly IDocumentStore _documentStore;

        public RavenDBEventStore(string ravenUrl)
        {
            _documentStore = new DocumentStore
            {
                Url = ravenUrl,                
                Conventions = CreateConventions()
            }.Initialise(); 
        }

        public RavenDBEventStore(DocumentStore externalDocumentStore)
        {
            externalDocumentStore.Conventions = CreateConventions();
            _documentStore = externalDocumentStore;            
        }

        private static DocumentConvention CreateConventions()
        {
            return new DocumentConvention
            {
                JsonContractResolver = new PropertiesOnlyContractResolver(),
                FindTypeTagName = x => "Events",
                NewDocumentETagGenerator = GenerateETag
            };
        }

        private static Guid? GenerateETag(object entity)
        {
            var sourcedEvent = entity as StoredEvent;
            if (sourcedEvent != null)
            {
                return Guid.NewGuid();
            }
            return null;
        }

        public CommittedEventStream ReadUntil(Guid id, long? maxVersion)
        {
            maxVersion = maxVersion ?? long.MaxValue;
            using (var session = _documentStore.OpenSession())
            {
                var storedEvents = session.Query<StoredEvent>()
                    .WaitForNonStaleResults()
                    .Where(x => x.EventSourceId == id)
                    .Where(x => x.EventSequence <= maxVersion)
                    .ToList().OrderBy(x => x.EventSequence);
                return new CommittedEventStream(storedEvents.Select(ToComittedEvent));
            }
        }

        public CommittedEventStream ReadFrom(Guid id, long minVersion)
        {
            using (var session = _documentStore.OpenSession())
            {
                var storedEvents = session.Query<StoredEvent>()
                    .WaitForNonStaleResults()
                    .Where(x => x.EventSourceId == id)
                    .Where(x => x.EventSequence >= minVersion)
                    .ToList().OrderBy(x => x.EventSequence);
                return new CommittedEventStream(storedEvents.Select(ToComittedEvent));
            }
        }

        private static CommittedEvent ToComittedEvent(StoredEvent x)
        {
            return new CommittedEvent(x.CommitId, x.EventIdentifier, x.EventSourceId,x.EventSequence, x.EventTimeStamp, x.Data, x.Version);
        }

        public void Store(UncommittedEventStream eventStream)
        {
            try
            {
                using (var session = _documentStore.OpenSession())
                {
                    session.UseOptimisticConcurrency = true;
                    foreach (var uncommittedEvent in eventStream)
                    {
                        session.Store(ToStoredEvent(eventStream.CommitId, uncommittedEvent));
                    }
                    session.SaveChanges();
                }
            }
            catch (Raven.Database.Exceptions.ConcurrencyException)
            {
                Guid sourceId = Guid.Empty;
                long version = 0;
                if (eventStream.HasSingleSource)
                {
                    sourceId = eventStream.SourceId;
                    version = eventStream.Sources.Single().CurrentVersion;
                }
                throw new ConcurrencyException(sourceId, version);
            }
        }

        private static StoredEvent ToStoredEvent(Guid commitId, UncommittedEvent uncommittedEvent)
        {
            return new StoredEvent
                       {
                           Id = uncommittedEvent.EventSourceId + "/" + uncommittedEvent.EventSequence,
                           EventIdentifier = uncommittedEvent.EventIdentifier,
                           EventTimeStamp = uncommittedEvent.EventTimeStamp,
                           Version = uncommittedEvent.EventVersion,
                           CommitId = commitId,
                           Data = uncommittedEvent.Payload,
                           EventSequence = uncommittedEvent.EventSequence,
                           EventSourceId = uncommittedEvent.EventSourceId,
                       };
        }
    }
}
