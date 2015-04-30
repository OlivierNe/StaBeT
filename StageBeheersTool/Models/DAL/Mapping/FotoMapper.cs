using System.Data.Entity.ModelConfiguration;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Mapping 
{
    public class FotoMapper : EntityTypeConfiguration<Foto>
    {
        public FotoMapper()
        {
            ToTable("Fotos");
            Property(f => f.ContentType).HasMaxLength(50).IsRequired();
            Property(f => f.FotoData).IsRequired();
        }
    }
}