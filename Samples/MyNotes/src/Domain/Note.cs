using System;
using Ncqrs;
using Events;

namespace Domain
{
    public class Note : MyNotesAggregateRoot
    {
        private String _text;

        private Note()
        {
            // Need a default ctor for Ncqrs.
        }

        public Note(String text)
        {
            var clock = NcqrsEnvironment.Get<IClock>();

            ApplyEvent(new NewNoteAdded
            {
                NoteId = Id,
                Text = text,
                CreationDate = clock.UtcNow()
            });
        }

        public void ChangeText(String newText)
        {
            ApplyEvent(new NoteTextChanged
            {
                NoteId = Id,
                NewText = newText
            });
        }

        protected void OnNewNoteAdded(NewNoteAdded e)
        {
            Id = e.NoteId;
            _text = e.Text;
        }

        protected void OnNoteTextChanged(NoteTextChanged e)
        {
            _text = e.NewText;
        }
    }
}
