using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing.Storage.WindowsAzure.Tests.Env;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Storage.WindowsAzure.Tests
{
    [TestClass]
    public class MainTests
    {

        public MainTests()
        {
            Env.Startup.Start();
        }

        [TestMethod]
        public void SniffForSmoke()
        {
            
            Guid smokeID = Guid.NewGuid();
            NcqrsEnvironment.Get<ICommandService>().Execute(new Env.CreateNoteCommand()
            {
                NoteId = smokeID,
                NoteText = "Hello world"
            });

            NcqrsEnvironment.Get<ICommandService>().Execute(new Env.ChangeNoteCommand()
            {
                NoteId = smokeID,
                NewNoteText = "Hello universe"
            });

            

        }

        [TestMethod]
        public void LoadItUp()
        {
            IList<Guid> ids = new List<Guid>();

            for (int i = 0; i < 5; i++)
            {
                Guid id = Guid.NewGuid();
                ids.Add(id);

                NcqrsEnvironment.Get<ICommandService>().Execute(new Env.CreateNoteCommand()
                {
                    NoteId = id,
                    NoteText = "Hello world " + i
                });    
            }


            foreach (Guid id in ids)
            {

                NcqrsEnvironment.Get<ICommandService>().Execute(new Env.ChangeNoteCommand()
                {
                    NoteId = id,
                    NewNoteText = "Hello solar system"
                });
            }

            foreach (Guid id in ids)
            {

                NcqrsEnvironment.Get<ICommandService>().Execute(new Env.ChangeNoteCommand()
                {
                    NoteId = id,
                    NewNoteText = "Hello galaxy"
                });
            }
        }
    }
}
