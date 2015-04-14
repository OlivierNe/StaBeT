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
            ToTable("Studenten");
            Property(student => student.Id).HasColumnName("student_id");
            Property(student => student.HogentEmail).IsRequired().HasMaxLength(100)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("HogentEmailIndex") { IsUnique = true }));
            Property(student => student.Email).HasMaxLength(50);
            Property(student => student.Familienaam).HasMaxLength(30);
            Property(student => student.Voornaam).HasMaxLength(20);
            Property(student => student.Telefoon).HasMaxLength(20);
            Property(student => student.Gsm).HasMaxLength(20);
            Property(student => student.Gemeente).HasMaxLength(30);
            Property(student => student.Straat).HasMaxLength(50);
            Property(student => student.Postcode).HasMaxLength(15);
            Property(student => student.FotoUrl).HasMaxLength(100);

            HasMany(student => student.Stages).WithRequired(stage => stage.Student).WillCascadeOnDelete(false);
            HasMany(student => student.VoorkeurStages).WithRequired(voorkeurStage => voorkeurStage.Student).WillCascadeOnDelete(true);
            HasOptional(student => student.Keuzepakket);
        }
    }
}