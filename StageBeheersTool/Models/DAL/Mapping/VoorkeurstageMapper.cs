using System.Data.Entity.ModelConfiguration;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class VoorkeurStageMapper : EntityTypeConfiguration<VoorkeurStage>
    {
        public VoorkeurStageMapper()
        {
            ToTable("VoorkeurStages");
            HasRequired(s => s.Stageopdracht);
            HasRequired(s => s.Student);
            HasKey(s => new { s. StudentId, s.StageopdrachtId });
        }
    }
}