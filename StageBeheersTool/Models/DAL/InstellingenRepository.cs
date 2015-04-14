using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL
{
    public class InstellingenRepository : IInstellingenRepository
    {
        private readonly StageToolDbContext _dbContext;
        private readonly DbSet<Instelling> _instellingen;

        public InstellingenRepository(StageToolDbContext ctx)
        {
            _dbContext = ctx;
            _instellingen = ctx.Instellingen;
        }

        public IQueryable<Instelling> FindAll()
        {
            return _instellingen;
        }

        public Instelling Find(string key)
        {
            return _instellingen.FirstOrDefault(instelling => instelling.Key == key);
        }

        public void AddOrUpdate(Instelling instelling)
        {
            _instellingen.AddOrUpdate(instelling);
            SaveChanges();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}