using StageBeheersTool.Models.Domain;
using System.Data.Entity;
using System.Linq;

namespace StageBeheersTool.Models.DAL
{
    public class SpecialisatieRepository : ISpecialisatieRepository
    {
        private readonly DbSet<Specialisatie> _specialisaties;

        public SpecialisatieRepository(StageToolDbContext ctx)
        {
            _specialisaties = ctx.Specialisaties;
        }

        public IQueryable<Specialisatie> FindAll()
        {
            return _specialisaties;
        }

        public Specialisatie FindBy(int id)
        {
            return _specialisaties.SingleOrDefault(s => s.Id == id);
        }
    }
}