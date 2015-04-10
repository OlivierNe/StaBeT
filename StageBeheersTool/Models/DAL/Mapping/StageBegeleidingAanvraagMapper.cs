using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class StagebegeleidingAanvraagMapper : EntityTypeConfiguration<StagebegeleidingAanvraag>
    {
        public StagebegeleidingAanvraagMapper()
        {
            ToTable("Stagebegeleiding_aanvragen");
            HasRequired(aanvraag => aanvraag.Stage);
            HasRequired(aanvraag => aanvraag.Begeleider);
        }
    }
}
