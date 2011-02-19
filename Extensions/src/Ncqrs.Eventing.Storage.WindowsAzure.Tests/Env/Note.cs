using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;

namespace Ncqrs.Eventing.Storage.WindowsAzure.Tests.Env
{
    public class Note : AggregateRootMappedByConvention
    {
        public string NoteText {get;private set;}


        public Note(Guid noteId, string noteText) : base(noteId)
        {
            if (noteText != null)
            {
                ApplyEvent(new NoteCreated
                {
                    NoteText = noteText
                });
            }
        }

        public Note()
        {
        }

        public void ChangeNoteText(string NewNoteText)
        {
            ApplyEvent(new NoteChanged
            {
                NewNoteText = NewNoteText
            });
            
        }

        protected void OnNoteCreated(NoteCreated e)
        {
            NoteText = e.NoteText;
        }

        protected void OnNoteChange(NoteChanged e)
        {
            NoteText = e.NewNoteText;
        }

        
    }
}
