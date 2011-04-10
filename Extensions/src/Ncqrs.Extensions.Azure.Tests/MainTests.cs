using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ncqrs.Commanding.ServiceModel;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using Ncqrs.Extensions.Azure.Storage;

namespace Ncqrs.Extensions.Azure.Tests
{
    [TestClass]
    public class MainTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            string prefix = Env.ConfigureTestEnvironment(true, true);

            IList<Guid> ids = new List<Guid>();
            for (int runs = 0; runs < 10; runs++)
            {
                Guid g = Guid.NewGuid();
                ids.Add(g);
                NcqrsEnvironment.Get<ICommandService>().Execute(new Notes.CreateNoteCommand()
                {
                    NoteId = g,
                    NoteText = "Hello world"
                });
            }

            foreach (Guid g in ids)
            {
                NcqrsEnvironment.Get<ICommandService>().Execute(new Notes.ChangeNoteCommand()
                {
                    NoteId = g,
                    NewNoteText = "Hello galaxy"
                });
            }

            foreach (Guid g in ids)
            {
                NcqrsEnvironment.Get<ICommandService>().Execute(new Notes.ChangeNoteCommand()
                {
                    NoteId = g,
                    NewNoteText = "Hello universe"
                });
            }

            foreach (Guid g in ids)
            {
                int foundEvents = new NcqrsEventStoreContext(g,
                    CloudStorageAccount.DevelopmentStorageAccount,
                    prefix).Events.ToList().Count();

                Assert.AreEqual(foundEvents, 3);
            }


            // Don't leave tables lying around if test passed.
            Microsoft.WindowsAzure.CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient().DeleteTable(prefix + "NcqrsEventStoreSource");
            Microsoft.WindowsAzure.CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient().DeleteTable(prefix + "NcqrsEventStore");
        }

    }
}
