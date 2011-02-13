using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.WindowsAzure.Tests.Env
{
    [Serializable]
    public class NoteCreated : SourcedEvent
    {
        Guid NoteId { get; set; }
        public string NoteText { get; set; }
    }
}
