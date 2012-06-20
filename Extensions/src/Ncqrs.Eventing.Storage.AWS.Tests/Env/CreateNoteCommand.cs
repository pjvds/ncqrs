using System;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Eventing.Storage.AWS.Tests.Env
{
    [MapsToAggregateRootConstructor(typeof(Note))]
    public class CreateNoteCommand : CommandBase
    {
        public Guid NoteId { get; set; }
        public string NoteText { get; set; }
    }
}
