using System;

namespace Ncqrs.Tests.Integration.Domain
{
    [Serializable]
    public class NoteTextChangedEvent
    {
        public String NewText
        {
            get;
            set;
        }
    }
}
