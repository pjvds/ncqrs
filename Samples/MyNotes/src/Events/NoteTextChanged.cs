using System;
using Ncqrs.Eventing.Sourcing;

namespace Events
{
    [Serializable]
    public class NoteTextChanged : SourcedEvent
    {
        public Guid NoteId
        {
            get; set;
        }

        public String NewText
        {
            get;
            set;
        }
    }
}
