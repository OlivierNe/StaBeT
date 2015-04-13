using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class AcademiejaarInstellingenMapper : EntityTypeConfiguration<AcademiejaarInstellingen>
    {
        public AcademiejaarInstellingenMapper()
        {
            HasKey(aj => aj.Academiejaar);
        }
    }
}
