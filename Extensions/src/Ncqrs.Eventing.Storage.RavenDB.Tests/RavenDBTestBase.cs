using System.IO;
using System.Reflection;
using log4net.Config;
using NUnit.Framework;
using Raven.Client.Document;

namespace Ncqrs.Eventing.Storage.RavenDB.Tests
{
    public abstract class RavenDBTestBase
    {
        protected DocumentStore _documentStore;
        private string path;

        [SetUp]
        public void SetUpDocumentStore()
        {
            XmlConfigurator.Configure();
            //_documentStore = ConnectToDocumentStore();
            _documentStore = NewDocumentStore();
        }

        [TearDown]
        public void TearDownDocumentStore()
        {
            _documentStore.Dispose();
            if (path != null)
            {
                Directory.Delete(path, true);
            }
        }

        private static DocumentStore ConnectToDocumentStore()
        {
            var documentStore = new DocumentStore
                                    {
                                        Url = "http://localhost:8080"
                                    };
            documentStore.Initialise();
            return documentStore;
        }

        private DocumentStore NewDocumentStore()
        {
            path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(RavenDBEventStoreTests)).CodeBase);
            path = Path.Combine(path, "TestDb").Substring(6);
            var documentStore = new DocumentStore
                                    {
                                        Configuration =
                                            {
                                                DataDirectory = path,
                                                RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true
                                            }
                                    };
            documentStore.Initialise();
            return documentStore;
        }
    }
}