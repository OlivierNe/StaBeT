using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class StageStudentRelatieMapper : EntityTypeConfiguration<StageStudentRelatie>
    {
        public StageStudentRelatieMapper()
        {
            this.ToTable("Student_Stage_Relaties");
            this.HasRequired(ssr => ssr.Student);
            this.HasRequired(ssr => ssr.Stage);
            this.HasKey(ssr => new { ssr.StudentId, ssr.StageId});

            //.Map(m =>
            //{
            //    m.MapLeftKey("Stageopdracht_id");
            //    m.MapRightKey("Student_id");
            //    m.ToTable("Student_Stageopdracht");
            //});
        }
    }
}