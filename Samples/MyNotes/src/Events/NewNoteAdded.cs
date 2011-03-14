using System;
using Ncqrs.Eventing.Sourcing;

namespace Events
{
    [Serializable]
    public class NewNoteAdded
    {
        public Guid NoteId
        {
            get; set;
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
