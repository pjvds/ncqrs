using System;
using Raven.Client;
using Raven.Client.Document;

namespace Ncqrs.Eventing.Storage.RavenDB
{
    public class RavenDBSnapshotStore : ISnapshotStore
    {
        private readonly IDocumentStore _documentStore;

        public RavenDBSnapshotStore(string ravenUrl)
        {
            _documentStore = new DocumentStore
            {
                Url = ravenUrl,                
                Conventions = CreateConventions()
            }.Initialise(); 
        }

        public RavenDBSnapshotStore(DocumentStore externalDocumentStore)
        {
            externalDocumentStore.Conventions = CreateConventions();
            _documentStore = externalDocumentStore;            
        }

        private static DocumentConvention CreateConventions()
        {
            return new DocumentConvention
            {
                JsonContractResolver = new PropertiesOnlyContractResolver(),
                FindTypeTagName = x => "Snapshots"
            };
        }
        
        public void SaveShapshot(ISnapshot source)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(new StoredSnaphot
                                  {
                                      Id = source.EventSourceId.ToString(),
                                      Data = source,
                                      EventSourceId = source.EventSourceId
                                  });
                session.SaveChanges();
            }
        }

        public ISnapshot GetSnapshot(Guid eventSourceId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var snapshot = session.Load<StoredSnaphot>(eventSourceId.ToString());
                return snapshot != null
                           ? (ISnapshot)snapshot.Data
                           : null;
            }
        }
    }
}