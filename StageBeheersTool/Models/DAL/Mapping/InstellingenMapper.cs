using System.Data.Entity.ModelConfiguration;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class InstellingenMapper : EntityTypeConfiguration<Instelling>
    {
        public InstellingenMapper()
        {
            ToTable("Instellingen");
            HasKey(instelling => instelling.Key);
            Property(instelling => instelling.Key).IsRequired();
            Property(instelling => instelling.Value).IsRequired().HasMaxLength(200);
        }
    }
}