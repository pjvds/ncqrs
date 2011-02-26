using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Events;
using Ncqrs;
using Ncqrs.Spec;
using Ncqrs.Spec.Fakes;
using NUnit.Framework;

namespace Tests
{
    /// <summary>
    /// This is an example of the OneEventTestFixture with event history
    /// </summary>
    /// <remarks>Use this type of test when the command results in exactly one event.
    /// </remarks>
    [Specification]
    public class when_changing_note_text : OneEventTestFixture<ChangeNoteText, NoteTextChanged>
    {

        public when_changing_note_text()
        {
            Configuration.Configure();
        }

        private DateTime now = DateTime.UtcNow;
        private const string OldNoteText = "Note text goes here";
        private const string NewNoteText = "New note text goes here";

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
            Assert.That(TheEvent.NewText, Is.EqualTo(NewNoteText));
        }

        [Then]
        public void it_should_change_the_right_note()
        {
            Assert.That(PublishedEvents.Single().EventSourceId, Is.EqualTo(EventSourceId));
        }

    }
}
