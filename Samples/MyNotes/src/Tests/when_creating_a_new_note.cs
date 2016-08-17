using System;
using System.Collections.Generic;
using Commands;
using Events;
using Ncqrs;
using Ncqrs.Spec;
using Ncqrs.Spec.Fakes;
using Xunit;

namespace Tests
{
    /// <summary>
    /// This is an example of the OneEventTestFixture without event history
    /// </summary>
    /// <remarks>Use this type of test when the command results in exactly one event.
    /// </remarks>
    public class when_creating_a_new_note : OneEventTestFixture<CreateNewNote, NewNoteAdded>, IClassFixture<ConfigurationFixture>
    {

        public when_creating_a_new_note(ConfigurationFixture configuration):base()
        {

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
            Assert.Equal(TheEvent.NoteId, EventSourceId);
        }

        [Then]
        public void the_new_note_should_have_the_correct_text()
        {
            Assert.Equal(TheEvent.Text, NoteText);
        }

        [Then]
        public void the_new_note_should_have_the_correct_creation_date()
        {
            Assert.Equal(TheEvent.CreationDate, now);
        }


    }
}
