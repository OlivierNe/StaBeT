using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class SpecialisatieMapper : EntityTypeConfiguration<Specialisatie>
    {
        public SpecialisatieMapper()
        {
            this.ToTable("Specialisaties");
            this.Property(s => s.Naam).IsRequired().HasMaxLength(50);
        }
    }
}