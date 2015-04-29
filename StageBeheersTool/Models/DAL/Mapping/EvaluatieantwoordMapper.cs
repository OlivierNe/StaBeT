using System.Data.Entity.ModelConfiguration;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class EvaluatieantwoordMapper: EntityTypeConfiguration<Evaluatieantwoord>
    {
        public EvaluatieantwoordMapper()
        {
            ToTable("Evaluatieantwoorden");
            HasRequired(antwoord => antwoord.Evaluatievraag);
        }
    }
}