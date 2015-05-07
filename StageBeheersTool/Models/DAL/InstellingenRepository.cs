using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL
{
    public class InstellingenRepository : IInstellingenRepository
    {
        private readonly StageToolDbContext _dbContext;
        private readonly DbSet<Instelling> _instellingen;
        private readonly DbSet<AcademiejaarInstellingen> _academiejaarInstellingen;

        public InstellingenRepository(StageToolDbContext ctx)
        {
            _dbContext = ctx;
            _instellingen = ctx.Instellingen;
            _academiejaarInstellingen = ctx.AcademiejarenInstellingen;
        }

        #region algemene instellingen
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
        #endregion

        #region academiejaar instellingen

        public void AddAcademiejaarInstelling(AcademiejaarInstellingen academiejaar)
        {
            _academiejaarInstellingen.AddOrUpdate(academiejaar);
            var stages = _dbContext.Stages.Where(stage => stage.Stageopdracht.Academiejaar == academiejaar.Academiejaar);
            foreach (var stage in stages)
            {
                stage.AcademiejaarInstellingen = academiejaar;
            }
            SaveChanges();
        }

        public AcademiejaarInstellingen FindAcademiejaarInstellingVanHuidig()
        {
            string huidigAcademiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return _academiejaarInstellingen.SingleOrDefault(aj => aj.Academiejaar.Equals(huidigAcademiejaar));
        }

        public AcademiejaarInstellingen FindAcademiejaarInstellingByAcademiejaar(string academiejaar)
        {
            return _academiejaarInstellingen.Find(academiejaar);
        }

        public IQueryable<AcademiejaarInstellingen> FindAllAcademiejaarInstellingen()
        {
            return _academiejaarInstellingen;
        }

        public void UpdateAcademiejaarInstelling(AcademiejaarInstellingen academiejaar)
        {
            _dbContext.Entry(academiejaar).State = EntityState.Modified;
            var stages = _dbContext.Stages.Where(stage => stage.Stageopdracht.Academiejaar == academiejaar.Academiejaar);
            foreach (var stage in stages)
            {
                stage.AcademiejaarInstellingen = academiejaar;
            }
            SaveChanges();
        }

        public void DeleteAcademiejaarInstelling(AcademiejaarInstellingen academiejaar)
        {
            _academiejaarInstellingen.Remove(academiejaar);
            SaveChanges();
        }

        #endregion

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}