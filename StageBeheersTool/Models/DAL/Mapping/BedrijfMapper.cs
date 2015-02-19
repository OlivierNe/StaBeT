using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
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
            this.Property(bedrijf => bedrijf.Naam).IsRequired().HasMaxLength(100);
            this.Property(bedrijf => bedrijf.Email).IsRequired().HasMaxLength(100);
            //this.Property(bedrijf => bedrijf.Adres.Gemeente).IsRequired().HasMaxLength(100);
            //this.Property(bedrijf => bedrijf.Straat).IsRequired().HasMaxLength(100);
            //this.Property(bedrijf => bedrijf.Postcode).IsRequired().IsFixedLength().HasMaxLength(4);
            //this.Property(bedrijf => bedrijf.Straatnummer).IsRequired();
            this.Property(bedrijf => bedrijf.Bereikbaarheid).IsRequired();
            this.Property(bedrijf => bedrijf.BedrijfsActiviteiten).IsRequired();
            this.HasMany(bedrijf => bedrijf.Contactpersonen).WithRequired().WillCascadeOnDelete(true);
            this.HasMany(bedrijf => bedrijf.Stageopdrachten).WithOptional();//
        }
    }
}