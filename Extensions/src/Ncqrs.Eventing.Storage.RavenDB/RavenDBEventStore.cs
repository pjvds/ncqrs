using System;
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

        public IEnumerable<ISourcedEvent> GetAllEvents(Guid id)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<StoredEvent>()
                    .WaitForNonStaleResults()
                    .Where(x => x.EventSourceId == id)
                    .ToList().OrderBy(x => x.EventSequence)
                    .Select(x => x.Data);                   
            }
        }

        public IEnumerable<ISourcedEvent> GetAllEventsSinceVersion(Guid id, long version)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<StoredEvent>()
                    .WaitForNonStaleResults()
                    .Where(x => x.EventSourceId == id)
                    .Where(x => x.EventSequence >= version)
                    .ToList().OrderBy(x => x.EventSequence)
                    .Select(x => x.Data);                   
            }
        }

        public void Save(IEventSource source)
        {
            try
            {
                using (var session = _documentStore.OpenSession())
                {
                    session.UseOptimisticConcurrency = true;
                    foreach (var sourcedEvent in source.GetUncommittedEvents())
                    {
                        session.Store(new StoredEvent
                        {
                            Data = sourcedEvent,
                            EventSequence = sourcedEvent.EventSequence,
                            EventSourceId = sourcedEvent.EventSourceId,
                            Id = sourcedEvent.EventSourceId + "/" + sourcedEvent.EventSequence
                        });
                    }
                    session.SaveChanges();
                }
            }
            catch (Raven.Database.Exceptions.ConcurrencyException)
            {
                throw new ConcurrencyException(source.EventSourceId, source.Version);
            }
        }        
    }
}
