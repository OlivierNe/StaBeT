using System.Data.Entity.ModelConfiguration;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class StudentVoorkeurStageMapper : EntityTypeConfiguration<StudentVoorkeurStage>
    {
        public StudentVoorkeurStageMapper()
        {
            ToTable("Student_voorkeurstages");
            this.HasRequired(s => s.Stageopdracht);
            this.HasRequired(s => s.Student);
            this.HasKey(s => new { s. StudentId, s.StageopdrachtId });
        }
    }
}