using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ncqrs.Eventing.Sourcing;

namespace MyNotes.Events
{
    [Serializable]
    public class NoteTextChanged
    {
        public string Text { get; set; }
    }
}
