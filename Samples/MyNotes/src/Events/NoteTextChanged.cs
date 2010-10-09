using System;
using Ncqrs.Eventing.Sourcing;

namespace Events
{
    [Serializable]
    public class NoteTextChanged : SourcedEvent
    {
        public Guid NoteId
        {
            get { return EventSourceId; }
        }

        public String NewText
        {
            get;
            set;
        }
    }
}
