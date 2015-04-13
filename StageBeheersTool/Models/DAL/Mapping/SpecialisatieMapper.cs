using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class SpecialisatieMapper : EntityTypeConfiguration<Specialisatie>
    {
        public SpecialisatieMapper()
        {
            ToTable("Specialisaties");
            Property(specialisie => specialisie.Id).HasColumnName("specialisatie_id");
            Property(specialisie => specialisie.Naam).IsRequired().HasMaxLength(50);
        }
    }
}
