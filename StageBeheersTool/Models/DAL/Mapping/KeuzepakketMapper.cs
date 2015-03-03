using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class KeuzepakketMapper : EntityTypeConfiguration<Keuzepakket>
    {
        public KeuzepakketMapper()
        {
            this.ToTable("Keuzepakketten");
            this.Property(k => k.Naam).IsRequired().HasMaxLength(50);
        }
    }
}