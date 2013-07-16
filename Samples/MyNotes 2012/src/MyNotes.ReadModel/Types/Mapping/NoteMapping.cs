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
    internal class NoteMapping : EntityTypeConfiguration<Types.Note>
    {
        public NoteMapping(string schema)
        {
            // Table key
            this.HasKey(note => note.Id).ToTable("Note", schema);

            // Mapped field(s)
            this.Property(note => note.Id).IsRequired().HasColumnName("Id").HasColumnOrder(1);
            this.Property(note => note.Text).IsRequired().HasColumnName("Text").HasColumnOrder(2);
            this.Property(note => note.CreationDate).IsRequired().HasColumnName("CreationDate").HasColumnOrder(3);
        }
    }
}
