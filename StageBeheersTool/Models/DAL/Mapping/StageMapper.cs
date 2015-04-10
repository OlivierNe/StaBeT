using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class StageMapper : EntityTypeConfiguration<Stage>
    {
        public StageMapper()
        {
            ToTable("Stages");
            HasRequired(stage => stage.Student);
            HasRequired(stage => stage.Stageopdracht);
            HasKey(stage => new { stage.StudentId, stage.StageopdrachtId });

        }
    }
}