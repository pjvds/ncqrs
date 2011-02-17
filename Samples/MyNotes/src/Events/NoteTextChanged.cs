using System;
using Ncqrs.Eventing.Sourcing;

namespace Events
{
    [Serializable]
    public class NoteTextChanged
    {
        public String NewText
        {
            get;
            set;
        }
    }
}
