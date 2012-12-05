using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Commands
{
    [MapsToAggregateRootMethod("MyProject.Domain.Note, Domain", "ChangeText")]
    public class ChangeNoteText : CommandBase
    {
        [AggregateRootId]
        public Guid NoteId
        {
            get; set;
        }

        public String NewText
        {
            get; set;
        }
    }
}
