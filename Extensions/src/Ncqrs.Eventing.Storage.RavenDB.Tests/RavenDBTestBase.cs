using System.IO;
using System.Reflection;
using log4net.Config;
using NUnit.Framework;
using Raven.Client.Document;
using Raven.Client.Embedded;

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
            if (_documentStore != null)
            {
                _documentStore.Dispose();
            }
        }

        private static DocumentStore ConnectToDocumentStore()
        {
            var documentStore = new DocumentStore
                                    {
                                        Url = "http://localhost:8080"
                                    };
            documentStore.Initialize();
            return documentStore;
        }

        private DocumentStore NewDocumentStore()
        {
            path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(RavenDBEventStoreTests)).CodeBase);
            path = Path.Combine(path, "TestDb").Substring(6);
            if (Directory.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Directory);
                Directory.Delete(path, true);
            }
            var documentStore = new EmbeddableDocumentStore
                                    {
                                        DataDirectory = path
                                    };
            documentStore.Initialize();
            return documentStore;
        }
    }
}