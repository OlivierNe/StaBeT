using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class BedrijfMapper : EntityTypeConfiguration<Bedrijf>
    {
        public BedrijfMapper()
        {
            this.ToTable("Bedrijven");
            this.Property(bedrijf => bedrijf.Naam).IsRequired().HasMaxLength(100)
              .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("BedrijfdsnaamIndex") { IsUnique = true }));
            this.Property(bedrijf => bedrijf.Email).IsRequired().HasMaxLength(100);
            this.HasMany(bedrijf => bedrijf.Contactpersonen).WithRequired();
            this.HasMany(bedrijf => bedrijf.Stageopdrachten).WithRequired(so => so.Bedrijf);
        }
    }
}