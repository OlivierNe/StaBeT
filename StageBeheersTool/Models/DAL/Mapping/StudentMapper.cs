using StageBeheersTool.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class StudentMapper : EntityTypeConfiguration<Student>
    {
        public StudentMapper()
        {
            this.ToTable("Studenten");
            this.Property(student => student.HogentEmail).IsRequired().HasMaxLength(200)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("HogentEmailIndex") { IsUnique = true }));
            
            //this.HasMany()
            //this.HasMany(student => student.VoorkeurStages).WithMany()
            //   .Map(m =>
            //   {
            //       m.MapLeftKey("student_id");
            //       m.MapRightKey("stageopdracht_id");
            //       m.ToTable("Student_VoorkeurStage");
            //   });
        }
    }
}