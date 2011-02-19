using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Tests.Integration.Domain
{
    [MapsToAggregateRootMethod("Ncqrs.Tests.Integration.Domain.Note, Ncqrs.Tests.Integration", "ChangeText")]
    public class ChangeNoteTextCommand : CommandBase
    {
        public ChangeNoteTextCommand()
        {            
        }

        public ChangeNoteTextCommand(Guid commandId) : base(commandId)
        {
        }

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
