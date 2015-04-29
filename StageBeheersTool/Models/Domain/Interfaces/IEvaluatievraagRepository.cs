using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IEvaluatievraagRepository
    {
        IQueryable<Evaluatievraag> FindAll();
        IQueryable<Evaluatievraag> FindByStagebezoek(int stagebezoek);
        Evaluatievraag FindById(int id);
    }
}
