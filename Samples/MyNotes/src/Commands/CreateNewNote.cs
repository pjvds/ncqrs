using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Commands
{
    [MapsToAggregateRootConstructor("MyProject.Domain.Note, Domain")]
    public class CreateNewNote : CommandBase
    {
        public Guid NoteId
        {
            get; set;
        }

        public String Text
        {
            get; set;
        }

        public CreateNewNote()
        {
        }

        public CreateNewNote(Guid noteId, string text)
        {
            NoteId = noteId;
            Text = text;
        }
    }
}