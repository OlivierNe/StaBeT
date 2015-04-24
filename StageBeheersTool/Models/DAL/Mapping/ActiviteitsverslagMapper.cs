using System.Data.Entity.ModelConfiguration;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class ActiviteitsverslagMapper : EntityTypeConfiguration<Activiteitsverslag>
    {
        public ActiviteitsverslagMapper()
        {
            ToTable("Activiteitsverslagen");
            HasKey(verslag => verslag.Id);
            Property(verslag => verslag.Id).HasColumnName("activiteitsverslag_id");
        }
    }
}