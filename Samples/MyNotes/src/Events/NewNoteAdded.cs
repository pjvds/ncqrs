using System;
using Ncqrs.Eventing.Sourcing;

namespace Events
{
    [Serializable]
    public class NewNoteAdded : SourcedEvent
    {
        public Guid NoteId
        {
            get { return EventSourceId; }
        }

        public String Text
        {
            get; set;
        }

        public DateTime CreationDate
        {
            get; set;
        }
    }
}
