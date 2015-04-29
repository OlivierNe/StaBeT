using System.Data.Entity.ModelConfiguration;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class EvaluatievraagMapper : EntityTypeConfiguration<Evaluatievraag>
    {
        public EvaluatievraagMapper()
        {
            ToTable("Evaluatievragen");
            Property(vraag => vraag.Vraag).IsRequired();
            Property(vraag => vraag.SoortVraag).IsRequired();
            Property(vraag => vraag.Voor).IsRequired().HasMaxLength(50);
        }
    }
}