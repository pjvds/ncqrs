using System;

namespace Ncqrs.Tests.Integration.Domain
{
    [Serializable]
    public class NewNoteAddedEvent
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
