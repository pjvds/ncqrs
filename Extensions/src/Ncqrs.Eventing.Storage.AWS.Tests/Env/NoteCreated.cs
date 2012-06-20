using System;
using Ncqrs.Eventing.Sourcing;

namespace Ncqrs.Eventing.Storage.AWS.Tests.Env
{
    [Serializable]
    public class NoteCreated : SourcedEvent
    {
        Guid NoteId { get; set; }
        public string NoteText { get; set; }
    }
}
