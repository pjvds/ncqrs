using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.AWS.Tests.Env
{
    [Serializable]
    public class NoteChanged : SourcedEvent
    {
        public Guid NoteId { get; set; }
        public string NewNoteText { get; set; }
    }
}
