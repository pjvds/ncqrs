using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace MyNotes.ReadModel.Types.Mapping
{
    internal class DailyStatisticsMapping : EntityTypeConfiguration<Types.DailyStatistics>
    {
        public DailyStatisticsMapping(string schema)
        {
            // Table key
            this.HasKey(dailyStatistics => dailyStatistics.Id).ToTable("DailyStatistics", schema);

            // Mapped field(s)
            this.Property(dailyStatistics => dailyStatistics.Id).IsRequired().HasColumnName("Id").HasColumnOrder(1);
            this.Property(dailyStatistics => dailyStatistics.Date).IsRequired().HasColumnName("Date").HasColumnOrder(2);
            this.Property(dailyStatistics => dailyStatistics.NewCount).IsRequired().HasColumnName("NewCount").HasColumnOrder(3);
            this.Property(dailyStatistics => dailyStatistics.EditCount).IsRequired().HasColumnName("EditCount").HasColumnOrder(4);
        }
    }
}
