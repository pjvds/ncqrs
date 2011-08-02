using System;
using Ncqrs;
using Events;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing.Snapshotting;
using Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot;

namespace MyProject.Domain
{
    [DynamicSnapshot]
    public class Note : AggregateRootMappedByConvention
    {
        private String _text;
        private DateTime _creationDate;

        public Note()
        {
            // Need a default ctor for Ncqrs.
            // Need a default public ctor for DynamicSnapshot.
        }

        public Note(Guid noteId, String text) : base(noteId)
        {
            var clock = NcqrsEnvironment.Get<IClock>();

            // Apply a NewNoteAdded event that reflects the
            // creation of this instance. The state of this
            // instance will be update in the handler of 
            // this event (the OnNewNoteAdded method).
            ApplyEvent(new NewNoteAdded
            {
                NoteId = noteId,
                Text = text,
                CreationDate = clock.UtcNow()
            });
        }

        public void ChangeText(String newText)
        {
            // Apply a NoteTextChanged event that reflects
            // the occurence of a text change. The state of this
            // instance will be update in the handler of 
            // this event (the NoteTextChanged method).
            ApplyEvent(new NoteTextChanged
            {
                NewText = newText
            });
        }

        // Event handler for the NewNoteAdded event. This method
        // is automaticly wired as event handler based on convension.
        protected void OnNewNoteAdded(NewNoteAdded e)
        {
            _text = e.Text;
            _creationDate = e.CreationDate;
        }

        // Event handler for the NoteTextChanged event. This method
        // is automaticly wired as event handler based on convension.
        protected void OnNoteTextChanged(NoteTextChanged e)
        {
            _text = e.NewText;
        }

    }

}
