using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class StageBegeleidingAanvraagMapper : EntityTypeConfiguration<StagebegeleidingAanvraag>
    {
        public StageBegeleidingAanvraagMapper()
        {
            this.HasRequired(sba => sba.Stage);
            this.HasRequired(sba => sba.Begeleider);
        }
    }
}