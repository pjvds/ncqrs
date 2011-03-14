using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;
using Ncqrs.Commanding;

namespace Ncqrs.Eventing.Storage.WindowsAzure.Tests.Env
{
    [MapsToAggregateRootMethod(typeof(Note), "ChangeNoteText")]
    public class ChangeNoteCommand : CommandBase
    {
        [AggregateRootId]
        public Guid NoteId { get; set; }
        public string NewNoteText { get; set; }
    }
}
