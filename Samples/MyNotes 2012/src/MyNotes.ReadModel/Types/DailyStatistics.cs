using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyNotes.ReadModel.Types
{
    public class DailyStatistics
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int NewCount { get; set; }
        public int EditCount { get; set; }
    }
}
