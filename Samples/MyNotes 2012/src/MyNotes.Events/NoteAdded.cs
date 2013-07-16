using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ncqrs.Eventing.Sourcing;

namespace MyNotes.Events
{
    [Serializable]
    public class NoteAdded
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
