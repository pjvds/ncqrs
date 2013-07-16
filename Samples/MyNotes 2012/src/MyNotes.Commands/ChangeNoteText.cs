using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace MyNotes.Commands
{
    [MapsToAggregateRootMethod("MyNotes.Domain.Note, MyNotes.Domain", "ChangeText")]
    public class ChangeNoteText : CommandBase
    {
        [AggregateRootId]
        public Guid Id { get; set; }
        public string Text { get; set; }
    }
}
