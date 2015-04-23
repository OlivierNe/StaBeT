using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
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
            Property(stage => stage.StudentId)
                .HasColumnName("student_id")
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(
                    new IndexAttribute("StageopdrachtStudentUniqueIndex", 1) { IsUnique = true }));
            Property(stage => stage.StageopdrachtId)
                .HasColumnName("stageopdracht_id")
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(
                    new IndexAttribute("StageopdrachtStudentUniqueIndex", 2) { IsUnique = true }));
            HasRequired(stage => stage.Student).WithMany(student => student.Stages).HasForeignKey(stage => stage.StudentId);
            HasRequired(stage => stage.Stageopdracht).WithMany(stageopdracht => stageopdracht.Stages)
                .HasForeignKey(stage => stage.StageopdrachtId);
            HasOptional(stage => stage.AcademiejaarInstellingen).WithMany().WillCascadeOnDelete(true);
            HasMany(stage => stage.Activiteitsverslagen).WithRequired(verslag => verslag.Stage);

        }
    }
}
