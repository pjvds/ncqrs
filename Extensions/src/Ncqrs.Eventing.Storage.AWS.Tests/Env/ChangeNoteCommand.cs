using System;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding;

namespace Ncqrs.Eventing.Storage.AWS.Tests.Env
{
    [MapsToAggregateRootMethod(typeof(Note), "ChangeNoteText")]
    public class ChangeNoteCommand : CommandBase
    {
        [AggregateRootId]
        public Guid NoteId { get; set; }
        public string NewNoteText { get; set; }
    }
}
