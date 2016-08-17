using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Events;
using Ncqrs;
using Ncqrs.Spec;
using Ncqrs.Spec.Fakes;
using Xunit;

namespace Tests
{
    /// <summary>
    /// This is an example of the OneEventTestFixture with event history
    /// </summary>
    /// <remarks>Use this type of test when the command results in exactly one event.
    /// </remarks>
    public class when_changing_note_text : OneEventTestFixture<ChangeNoteText, NoteTextChanged>, IClassFixture<ConfigurationFixture>
    {

        private DateTime now = DateTime.UtcNow;
        private const string OldNoteText = "Note text goes here";
        private const string NewNoteText = "New note text goes here";

        public when_changing_note_text(ConfigurationFixture configuration):base()
        {

        }

        protected override void RegisterFakesInConfiguration(EnvironmentConfigurationWrapper configuration)
        {
            var clock = new FrozenClock(now);
            configuration.Register<IClock>(clock);
        }

        protected override IEnumerable<object> GivenEvents()
        {
            yield return new NewNoteAdded()
                             {
                                 CreationDate = now,
                                 NoteId = EventSourceId,
                                 Text = OldNoteText
                             };
        }

        protected override ChangeNoteText WhenExecuting()
        {
            return new ChangeNoteText()
                       {
                           NoteId = EventSourceId,
                           NewText = NewNoteText
                       };
        }

        [Then]
        public void it_should_change_the_note_text()
        {
            Assert.Equal(TheEvent.NewText, NewNoteText);
        }

        [Then]
        public void it_should_change_the_right_note()
        {
            Assert.Equal(PublishedEvents.Single().EventSourceId, EventSourceId);
        }

    }
}
