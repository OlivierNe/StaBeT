using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL
{
    public class StageRepository : IStageRepository
    {
        private readonly StageToolDbContext _dbContext;
        private readonly DbSet<Stage> _stages;


        public StageRepository(StageToolDbContext dbContext)
        {
            _dbContext = dbContext;
            _stages = dbContext.Stages;
        }

        public Stage FindById(int id)
        {
            return _stages.Find(id);
        }

        public IQueryable<Stage> FindAll()
        {
            return _stages;
        }

        public IQueryable<Stage> FindAllVanHuidigAcademiejaar()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return _stages.Where(stage => stage.Stageopdracht.Academiejaar == academiejaar).OrderByDescending(stage => stage.Id);
        }
        
        public void Update(Stage stage)
        {
            var teUpdatenStage = FindById(stage.Id);
            if (teUpdatenStage == null) return;
            teUpdatenStage.Semester = stage.Semester;
            teUpdatenStage.AangepasteStageperiode = stage.AangepasteStageperiode;
            if (teUpdatenStage.AangepasteStageperiode)
            {
                teUpdatenStage.Begindatum = stage.Begindatum;
                teUpdatenStage.Einddatum = stage.Einddatum;
            }
            else
            {
                teUpdatenStage.Begindatum = null;
                teUpdatenStage.Einddatum = null;
            }
            SaveChanges();
        }
        
        public void SaveChanges()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                string message = String.Empty;
                foreach (var eve in e.EntityValidationErrors)
                {

                    message +=
                        String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.GetValidationResult());
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message +=
                            String.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new ApplicationException("" + message);
            }
        }
       
    }
}