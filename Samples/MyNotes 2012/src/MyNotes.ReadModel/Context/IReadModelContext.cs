using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;

namespace MyNotes.ReadModel.Context
{
    public interface IReadModelContext
    {
        DbSet<Types.DailyStatistics> DailyStatistics { get; set; }
        DbSet<Types.Note> Notes { get; set; }
    }
}
