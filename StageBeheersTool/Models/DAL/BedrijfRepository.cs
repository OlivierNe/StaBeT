using System.Data.Entity.Infrastructure;
using MySql.Data.MySqlClient;
using StageBeheersTool.Models.Domain;
using System;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Validation;

namespace StageBeheersTool.Models.DAL
{
    public class BedrijfRepository : IBedrijfRepository
    {
        private readonly StageToolDbContext _dbContext;
        private readonly DbSet<Bedrijf> _bedrijven;

        public BedrijfRepository(StageToolDbContext ctx)
        {
            _dbContext = ctx;
            _bedrijven = ctx.Bedrijven;
        }

        public void Add(Bedrijf bedrijf)
        {
            try
            {
                _bedrijven.Add(bedrijf);
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException.InnerException as MySqlException;
                if (sqlException != null && sqlException.Number == 1062)
                {
                    throw new ApplicationException(string.Format(Resources.ErrorCreateBedrijf, bedrijf.Email));
                }
                throw;
            }
        }

        public IQueryable<Bedrijf> FindAll()
        {
            return _bedrijven.OrderBy(b => b.Naam);
        }


        public Bedrijf FindByEmail(string email)
        {
            return _bedrijven
                .SingleOrDefault(bedrijf => bedrijf.Email == email);
        }

        public Bedrijf FindById(int id)
        {
            return _bedrijven.SingleOrDefault(bedrijf => bedrijf.Id == id);
        }

        public void Update(Bedrijf bedrijf)
        {
            var teUpdatenBedrijf = FindById(bedrijf.Id);
            if (teUpdatenBedrijf == null)
                return;
            teUpdatenBedrijf.Naam = bedrijf.Naam;
            teUpdatenBedrijf.Gemeente = bedrijf.Gemeente;
            teUpdatenBedrijf.Postcode = bedrijf.Postcode;
            teUpdatenBedrijf.Straat = bedrijf.Straat;
            teUpdatenBedrijf.Bereikbaarheid = bedrijf.Bereikbaarheid;
            teUpdatenBedrijf.Bedrijfsactiviteiten = bedrijf.Bedrijfsactiviteiten;
            teUpdatenBedrijf.Website = bedrijf.Website;
            teUpdatenBedrijf.Telefoon = bedrijf.Telefoon;
            SaveChanges();
        }

        public void Delete(Bedrijf bedrijf)
        {
            try
            {
                _bedrijven.Remove(bedrijf);
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException.InnerException as MySqlException;
                if (sqlException != null && sqlException.Number == 1451)
                {
                    throw new ApplicationException(Resources.ErrorDeleteBedrijf);
                }
                throw;
            }
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