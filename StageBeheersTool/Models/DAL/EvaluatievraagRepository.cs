using System.Data.Entity;
using System.Linq;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL
{
    public class EvaluatievraagRepository : IEvaluatievraagRepository
    {
        private readonly StageToolDbContext _dbContext;
        private readonly DbSet<Evaluatievraag> _evaluatievragen;

        public EvaluatievraagRepository(StageToolDbContext dbContext)
        {
            _dbContext = dbContext;
            _evaluatievragen = _dbContext.Evaluatievragen;
        }

        public IQueryable<Evaluatievraag> FindAll()
        {
            return _evaluatievragen.OrderBy(vraag => vraag.Volgorde); ;
        }

        public IQueryable<Evaluatievraag> FindByStagebezoek(int stagebezoek)
        {
            return _evaluatievragen.Where(vraag => vraag.Stagebezoek == stagebezoek).OrderBy(vraag => vraag.Volgorde);
        }

        public Evaluatievraag FindById(int id)
        {
            return _evaluatievragen.Find(id);
        }
    }
}