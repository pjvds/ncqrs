using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Extensions.Azure.Tests.Notes
{
    [Serializable]
    public class NoteChanged : SourcedEvent
    {
        public Guid NoteId { get; set; }
        public string NewNoteText { get; set; }
    }
}
