using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class BedrijfMapper : EntityTypeConfiguration<Bedrijf>
    {
        public BedrijfMapper()
        {
            this.ToTable("Bedrijven");
            this.Property(bedrijf => bedrijf.Email).IsRequired().HasMaxLength(200);

            this.Property(bedrijf => bedrijf.Naam).IsRequired().HasMaxLength(100);
            this.Property(bedrijf => bedrijf.Email).IsRequired().HasMaxLength(100);
            this.HasMany(bedrijf => bedrijf.Contactpersonen).WithRequired();
            this.HasMany(bedrijf => bedrijf.Stageopdrachten).WithRequired(so => so.Bedrijf);
        }
    }
}