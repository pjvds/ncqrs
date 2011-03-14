using System;
using System.Collections.Generic;
using Commands;
using Events;
using Ncqrs;
using Ncqrs.Spec;
using Ncqrs.Spec.Fakes;
using NUnit.Framework;

namespace Tests
{
    /// <summary>
    /// This is an example of the OneEventTestFixture without event history
    /// </summary>
    /// <remarks>Use this type of test when the command results in exactly one event.
    /// </remarks>
    [Specification]
    public class when_creating_a_new_note : OneEventTestFixture<CreateNewNote, NewNoteAdded>
    {

        public when_creating_a_new_note()
        {
            Configuration.Configure();
        }

        private DateTime now = DateTime.UtcNow;
        private const string NoteText = "Note text goes here";

        protected override void RegisterFakesInConfiguration(EnvironmentConfigurationWrapper configuration)
        {
            var clock = new FrozenClock(now);
            configuration.Register<IClock>(clock);
        }

        protected override IEnumerable<object> GivenEvents()
        {
            return new object[0];
        }

        protected override CreateNewNote WhenExecuting()
        {
            return new CreateNewNote(EventSourceId, NoteText);
        }

        [Then]
        public void the_new_note_should_have_the_correct_note_id()
        {
            Assert.That(TheEvent.NoteId, Is.EqualTo(EventSourceId));
        }

        [Then]
        public void the_new_note_should_have_the_correct_text()
        {
            Assert.That(TheEvent.Text, Is.EqualTo(NoteText));
        }

        [Then]
        public void the_new_note_should_have_the_correct_creation_date()
        {
            Assert.That(TheEvent.CreationDate, Is.EqualTo(now));
        }


    }
}
