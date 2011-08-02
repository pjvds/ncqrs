using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Tests.Integration.Domain
{
    [MapsToAggregateRootConstructor("Ncqrs.Tests.Integration.Domain.Note, Ncqrs.Tests.Integration")]
    public class CreateNewNoteCommand : CommandBase
    {
        public CreateNewNoteCommand()
        {            
        }

        public CreateNewNoteCommand(Guid commandId)
            : base(commandId)
        {
        }

        public Guid NoteId
        {
            get; set;
        }

        public String Text
        {
            get; set;
        }
    }
}