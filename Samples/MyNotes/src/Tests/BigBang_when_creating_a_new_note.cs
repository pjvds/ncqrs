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
    /// This is an example of the BigBangTestFixture
    /// </summary>
    /// <remarks>
    /// Use this fixture when the command results in multiple events
    /// </remarks>
    [Specification]
    public class BigBang_when_creating_a_new_note : BigBangTestFixture<CreateNewNote>
    {

        public BigBang_when_creating_a_new_note()
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

        private NewNoteAdded NewNoteAddedEvent
        {
            get { return PublishedEvents.Select(e => e.Payload).OfType<NewNoteAdded>().Single(); }
        }

        [Then]
        public void the_new_note_should_have_the_correct_note_id()
        {
            Assert.That(NewNoteAddedEvent.NoteId, Is.EqualTo(EventSourceId));
        }

        [Then]
        public void the_new_note_should_have_the_correct_text()
        {
            Assert.That(NewNoteAddedEvent.Text, Is.EqualTo(NoteText));
        }

        [Then]
        public void the_new_note_should_have_the_correct_creation_date()
        {
            Assert.That(NewNoteAddedEvent.CreationDate, Is.EqualTo(now));
        }

        [Then]
        public void it_should_not_throw()
        {
            Assert.That(CaughtException, Is.EqualTo(null));
        }

        [Then]
        public void it_should_do_no_more()
        {
            Assert.That(PublishedEvents.Count(), Is.EqualTo(1));
        }

    }
}
