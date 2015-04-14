using StageBeheersTool.Models.Domain;
using System.Data.Entity;
using System.Linq;

namespace StageBeheersTool.Models.DAL
{
    public class KeuzepakketRepository : IKeuzepakketRepository
    {

        private readonly DbSet<Keuzepakket> _keuzepakketten;

        public KeuzepakketRepository(StageToolDbContext ctx)
        {
            _keuzepakketten = ctx.Keuzepakketten;
        }

        public IQueryable<Keuzepakket> FindAll()
        {
            return _keuzepakketten;
        }

        public Keuzepakket FindBy(int id)
        {
            return _keuzepakketten.SingleOrDefault(k => k.Id == id);
        }
    }
}