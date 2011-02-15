using System;
using System.IO;
using EventStore.Serialization;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Eventing.Storage;
using Ncqrs.Eventing.Storage.JOliver;
using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;
using Ncqrs.Tests.Integration.Domain;
using NUnit.Framework;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Tests.Integration
{
    [TestFixture]
    public class Idempotence
    {
        [Test]
        public void Executing_same_command_twice_will_have_same_result_as_once()
        {
            var commandService = new CommandService();
            commandService.RegisterExecutorsInAssembly(typeof(Idempotence).Assembly);
            NcqrsEnvironment.SetDefault(BuildEventStore());

            var noteId = Guid.NewGuid();
            var createNewCommand = new CreateNewNoteCommand(noteId, "Note One");
            commandService.Execute(createNewCommand);

            var updateCommand = new ChangeNoteTextCommand() {NoteId = noteId, NewText = "Note One Modified"};
            commandService.Execute(updateCommand);
        }

        [SetUp]
        public void CopyDatabase()
        {
            File.Copy("NcqrsIntegrationTestsClean.sdf", "NcqrsIntegrationTests.sdf", true);
        }

        private static IEventStore BuildEventStore()
        {
            var factory = new AbsoluteOrderingSqlPersistenceFactory("EventStore", new BinarySerializer());
            var streamPersister = factory.Build();
            streamPersister.Initialize();
            var store = new JoesEventStoreAdapter(streamPersister);
            return store;
        }
        
    }
}