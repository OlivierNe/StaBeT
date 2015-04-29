
namespace StageBeheersTool.Models.Domain
{
    public class Evaluatieantwoord
    {
        public int Id { get; set; }
        public string Antwoord { get; set; }
        public virtual Evaluatievraag Evaluatievraag { get; set; }
        public virtual Stage Stage { get; set; }
    }
}