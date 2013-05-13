using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;

namespace MyNotes.ReadModel.Context
{
    public class ReadModelContext : DbContext, IReadModelContext
    {
        private readonly string readModelSchema;

        public ReadModelContext() : this("MyNotes Read Model", null) { }
        public ReadModelContext(string nameOrConnectionString, string schema) : base(nameOrConnectionString)
        {
            this.readModelSchema = schema;
        }

        public DbSet<Types.DailyStatistics> DailyStatistics { get; set; }
        public DbSet<Types.Note> Notes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new Types.Mapping.NoteMapping(this.readModelSchema));
            modelBuilder.Configurations.Add(new Types.Mapping.DailyStatisticsMapping(this.readModelSchema));

            Database.SetInitializer<ReadModelContext>(null);
        }
    }
}
