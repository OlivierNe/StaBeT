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
            ToTable("Bedrijven");
            Property(bedrijf => bedrijf.Id).HasColumnName("bedrijf_id");
            Property(bedrijf => bedrijf.Email).IsRequired().HasMaxLength(50)
             .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("BedrijfEmailIndex") { IsUnique = true }));
            Property(bedrijf => bedrijf.Naam).IsRequired().HasMaxLength(100);
            Property(bedrijf => bedrijf.Email).IsRequired().HasMaxLength(50);
            Property(bedrijf => bedrijf.Telefoon).HasMaxLength(20);
            Property(bedrijf => bedrijf.Website).HasMaxLength(100);
            Property(bedrijf => bedrijf.Gemeente).HasMaxLength(30);
            Property(bedrijf => bedrijf.Straat).HasMaxLength(100);
            Property(bedrijf => bedrijf.Postcode).HasMaxLength(15);
            Property(bedrijf => bedrijf.Bereikbaarheid).HasMaxLength(100);
            Property(bedrijf => bedrijf.Bedrijfsactiviteiten).HasMaxLength(200);

            HasMany(bedrijf => bedrijf.Contactpersonen).WithRequired(cp => cp.Bedrijf).WillCascadeOnDelete(true);
            HasMany(bedrijf => bedrijf.Stageopdrachten).WithRequired(so => so.Bedrijf).WillCascadeOnDelete(false);
        }
    }
}
