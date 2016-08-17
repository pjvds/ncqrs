using System;
using Raven.Client;
using Raven.Client.Document;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Raven.Imports.Newtonsoft.Json.Converters;

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
                Conventions = SetConventions(new DocumentConvention())
            }.Initialize(); 
        }

        public RavenDBSnapshotStore(IDocumentStore externalDocumentStore)
        {
            SetConventions(externalDocumentStore.Conventions);
            _documentStore = externalDocumentStore;            
        }

        private DocumentConvention SetConventions(DocumentConvention convention)
        {
            convention.JsonContractResolver = new PropertiesOnlyContractResolver();
            convention.FindTypeTagName = x => "Snapshots";
            convention.CustomizeJsonSerializer = serializer => {
                serializer.Converters.Add(new VersionConverter());
            };
            return convention;
        }

        public Snapshot GetSnapshot(Guid eventSourceId, long maxVersion)
        {
            using (var session = _documentStore.OpenSession())
            {
                var snapshot = session.Load<StoredSnaphot>(eventSourceId.ToString());
                if (snapshot == null)
                {
                    return null;
                }
                return snapshot.Version <= maxVersion
                           ? new Snapshot(eventSourceId, snapshot.Version, snapshot.Data)
                           : null;
            }
        }

        public void SaveSnapshot(Snapshot source)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(new StoredSnaphot
                {
                    Id = source.EventSourceId.ToString(),
                    Data = source.Payload,
                    EventSourceId = source.EventSourceId,
                    Version = source.Version
                });

                session.SaveChanges();
            }
        }
    }
}