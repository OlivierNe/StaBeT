using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class StageMapper : EntityTypeConfiguration<Stage>
    {
        public StageMapper()
        {
            ToTable("Stages");
            Property(stage => stage.Id).HasColumnName("stage_id");
            HasRequired(stage => stage.Student);
            HasRequired(stage => stage.Stageopdracht);
        }
    }
}
