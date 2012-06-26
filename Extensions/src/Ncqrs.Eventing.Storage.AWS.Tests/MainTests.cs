using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing.Storage.AWS.Tests.Env;

namespace Ncqrs.Eventing.Storage.AWS.Tests
{
    [TestClass]
    public class MainTests
    {
        public MainTests()
        {
            Startup.Start();
        }

        [TestMethod]
        public void SniffForSmoke()
        {
            Guid smokeID = Guid.NewGuid();
            NcqrsEnvironment.Get<ICommandService>().Execute(new CreateNoteCommand
                                                                {
                                                                    NoteId = smokeID,
                                                                    NoteText = "Hello world"
                                                                });

            NcqrsEnvironment.Get<ICommandService>().Execute(new ChangeNoteCommand
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

                NcqrsEnvironment.Get<ICommandService>().Execute(new CreateNoteCommand
                                                                    {
                                                                        NoteId = id,
                                                                        NoteText = "Hello world " + i
                                                                    });
            }


            foreach (Guid id in ids)
            {
                NcqrsEnvironment.Get<ICommandService>().Execute(new ChangeNoteCommand
                                                                    {
                                                                        NoteId = id,
                                                                        NewNoteText = "Hello solar system"
                                                                    });
            }

            foreach (Guid id in ids)
            {
                NcqrsEnvironment.Get<ICommandService>().Execute(new ChangeNoteCommand
                                                                    {
                                                                        NoteId = id,
                                                                        NewNoteText = "Hello galaxy"
                                                                    });
            }
        }
    }
}