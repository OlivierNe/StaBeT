using System.Collections.Generic;
using System.Linq;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.ViewModels
{
    public class EvaluatieCreateVM
    {
        public int Stagebezoek { get; set; }
        public int StageId { get; set; }
        public IList<Evaluatieantwoord> Evaluatieantwoorden { get; set; }

        public EvaluatieCreateVM(IEnumerable<Evaluatievraag> evaluatievragen,
            IList<Evaluatieantwoord> evaluatieantwoorden, int stagebezoek, int stageId)
        {
            Evaluatieantwoorden = new List<Evaluatieantwoord>();
            foreach (var evaluatievraag in evaluatievragen)
            {
                var evaluatieantwoord = evaluatieantwoorden.FirstOrDefault(a => a.Evaluatievraag.Equals(evaluatievraag)) ??
                    new Evaluatieantwoord { Evaluatievraag = evaluatievraag };
                Evaluatieantwoorden.Add(evaluatieantwoord);
            }
            Stagebezoek = stagebezoek;
            StageId = stageId;
        }

        public EvaluatieCreateVM() { }
    }
}