using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace MyNotes.Commands
{
    [MapsToAggregateRootConstructor("MyNotes.Domain.Note, MyNotes.Domain")]
    public class CreateNewNote : CommandBase
    {
        public CreateNewNote() { }
        public CreateNewNote(Guid id, string text)
        {
            this.Id = id;
            this.Text = text;
        }

        public Guid Id{get;set;}
        public string Text{get;set;}
    }
}
