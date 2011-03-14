using System;
using Ncqrs.Domain;
using Ncqrs.Eventing.Sourcing.Snapshotting;

namespace Ncqrs.Tests.Integration.Domain
{
    public class Note : AggregateRootMappedByConvention, ISnapshotable<NoteSnapshot>
    {
        private String _text;
        private DateTime _creationDate;

        private Note()
        {
            // Need a default ctor for Ncqrs.
        }

        public Note(Guid noteId, String text) : base(noteId)
        {
            var clock = NcqrsEnvironment.Get<IClock>();

            ApplyEvent(new NewNoteAddedEvent
            {
                NoteId = noteId,
                Text = text,
                CreationDate = clock.UtcNow()
            });
        }

        public string Text
        {
            get { return _text; }
        }

        public void ChangeText(String newText)
        {
            ApplyEvent(new NoteTextChangedEvent
            {
                NewText = newText
            });
        }

        protected void OnNewNoteAdded(NewNoteAddedEvent e)
        {
            _text = e.Text;
            _creationDate = e.CreationDate;
        }

        protected void OnNoteTextChanged(NoteTextChangedEvent e)
        {
            _text = e.NewText;
        }
        
        public NoteSnapshot CreateSnapshot()
        {
            return new NoteSnapshot
                       {
                           Text = Text,
                           CreationDate = _creationDate
                       };
        }

        public void RestoreFromSnapshot(NoteSnapshot snapshot)
        {
            _text = snapshot.Text;
            _creationDate = snapshot.CreationDate;
        }
    }
    [Serializable]
    public class NoteSnapshot
    {
        public string Text { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
