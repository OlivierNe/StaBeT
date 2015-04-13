using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class KeuzepakketMapper : EntityTypeConfiguration<Keuzepakket>
    {
        public KeuzepakketMapper()
        {
            ToTable("Keuzepakketten");
            Property(keuzepakket => keuzepakket.Id).HasColumnName("keuzepakket_id");
            Property(keuzepakket => keuzepakket.Naam).IsRequired().HasMaxLength(50);
        }
    }
}