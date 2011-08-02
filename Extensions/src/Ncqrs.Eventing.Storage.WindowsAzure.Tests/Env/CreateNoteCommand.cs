using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Commanding;
using Ncqrs.Commanding.CommandExecution.Mapping.Attributes;

namespace Ncqrs.Eventing.Storage.WindowsAzure.Tests.Env
{
    [MapsToAggregateRootConstructor(typeof(Note))]
    public class CreateNoteCommand : CommandBase
    {
        public Guid NoteId {get;set;}
        public string NoteText {get;set;}
    }
}
