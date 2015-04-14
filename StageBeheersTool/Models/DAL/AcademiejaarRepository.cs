using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Migrations;

namespace StageBeheersTool.Models.DAL
{
    public class AcademiejaarRepository : IAcademiejaarRepository
    {
        private readonly StageToolDbContext _dbContext;
        private readonly DbSet<AcademiejaarInstellingen> _academiejarenInstellingen;

        public AcademiejaarRepository(StageToolDbContext ctx)
        {
            _dbContext = ctx;
            _academiejarenInstellingen = ctx.AcademiejarenInstellingen;
        }

        public void Add(AcademiejaarInstellingen academiejaar)
        {
            _academiejarenInstellingen.AddOrUpdate(academiejaar);
            SaveChanges();
        }

        public AcademiejaarInstellingen FindVanHuidigAcademiejaar()
        {
            string huidigAcademiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return _academiejarenInstellingen.SingleOrDefault(aj => aj.Academiejaar.Equals(huidigAcademiejaar));
        }

        public AcademiejaarInstellingen FindByAcademiejaar(string academiejaar)
        {
            return _academiejarenInstellingen.Find(academiejaar);
        }

        public IQueryable<AcademiejaarInstellingen> FindAll()
        {
            return _academiejarenInstellingen;
        }

        public void Update(AcademiejaarInstellingen academiejaar)
        {
            _dbContext.Entry(academiejaar).State = EntityState.Modified;
            SaveChanges();
        }

        public void Delete(AcademiejaarInstellingen academiejaar)
        {
            _academiejarenInstellingen.Remove(academiejaar);
            SaveChanges();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

    }
}