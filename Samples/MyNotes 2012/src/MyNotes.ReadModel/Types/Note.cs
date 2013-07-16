using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNotes.ReadModel.Types
{
    public class Note
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
