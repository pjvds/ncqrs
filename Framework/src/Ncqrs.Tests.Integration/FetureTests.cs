using System;
using Ncqrs.Commanding.ServiceModel;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Storage;
using Ncqrs.Tests.Integration.Domain;
using NUnit.Framework;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using FluentAssertions;

namespace Ncqrs.Tests.Integration
{
    [TestFixture]
    public abstract class FetureTests
    {
        [Test]
        public void Snapshotting_should_persist_state()
        {
            InitializeEnvironment();
            var commandService = new CommandService();
            commandService.RegisterExecutorsInAssembly(typeof(FetureTests).Assembly);
            NcqrsEnvironment.SetDefault<ISnapshottingPolicy>(new SimpleSnapshottingPolicy(2));

            var noteId = Guid.NewGuid();
            var createNewCommand = new CreateNewNoteCommand {NoteId = noteId, Text = "Note One"};
            commandService.Execute(createNewCommand);

            var updateCommand = new ChangeNoteTextCommand() {NoteId = noteId, NewText = "Note One Modified"};
            commandService.Execute(updateCommand);

            updateCommand = new ChangeNoteTextCommand() { NoteId = noteId, NewText = "Note One Modified Once Again" };
            commandService.Execute(updateCommand);

            updateCommand = new ChangeNoteTextCommand() { NoteId = noteId, NewText = "Note One Modified And Again" };
            commandService.Execute(updateCommand);

            var uowFactory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var uow = uowFactory.CreateUnitOfWork(Guid.NewGuid()))
            {
                var note = (Note) uow.GetById(typeof (Note), noteId, null);
                note.Text.Should().Be("Note One Modified And Again");
            }
        }

        [Test]
        public void Sending_command_with_same_command_id_as_thre_previous_command_should_have_no_effect()
        {
            InitializeEnvironment();
            var commandService = new CommandService();
            commandService.RegisterExecutorsInAssembly(typeof(FetureTests).Assembly);
            NcqrsEnvironment.SetDefault<ISnapshottingPolicy>(new SimpleSnapshottingPolicy(2));

            var noteId = Guid.NewGuid();
            var updateCommandId = Guid.NewGuid();
            var createNewCommand = new CreateNewNoteCommand(Guid.NewGuid()) { NoteId = noteId, Text = "Note One" };
            commandService.Execute(createNewCommand);

            var updateCommand = new ChangeNoteTextCommand(updateCommandId) { NoteId = noteId, NewText = "Note One Modified" };
            commandService.Execute(updateCommand);

            updateCommand = new ChangeNoteTextCommand(updateCommandId) { NoteId = noteId, NewText = "Note One Modified Once Again" };
            commandService.Execute(updateCommand);

            var uowFactory = NcqrsEnvironment.Get<IUnitOfWorkFactory>();
            using (var uow = uowFactory.CreateUnitOfWork(Guid.NewGuid()))
            {
                var note = (Note)uow.GetById(typeof(Note), noteId, null);
                note.Text.Should().Be("Note One Modified");
            }
        }

        protected abstract void InitializeEnvironment();
    }
}