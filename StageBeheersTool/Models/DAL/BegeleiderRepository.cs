using System.Data.Entity.Infrastructure;
using MySql.Data.MySqlClient;
using StageBeheersTool.Models.Domain;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace StageBeheersTool.Models.DAL
{
    public class BegeleiderRepository : IBegeleiderRepository
    {
        private readonly DbSet<Begeleider> _begeleiders;
        private readonly StageToolDbContext _dbContext;

        public BegeleiderRepository(StageToolDbContext ctx)
        {
            _dbContext = ctx;
            _begeleiders = ctx.Begeleiders;
        }

        public void Add(Begeleider begeleider)
        {
            try
            {
                _begeleiders.Add(begeleider);
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException.InnerException as MySqlException;
                if (sqlException != null && sqlException.Number == 1062)
                {
                    throw new ApplicationException(string.Format(Resources.ErrorBegeleiderCreateHogentEmailBestaatAl, begeleider.HogentEmail));
                }
                throw;
            }
        }

        public IQueryable<Begeleider> FindAll()
        {
            return _begeleiders.OrderBy(b => b.Familienaam);
        }

        public Begeleider FindByEmail(string hoGentEmail)
        {
            return _begeleiders.SingleOrDefault(b => b.HogentEmail == hoGentEmail);
        }

        public Begeleider FindById(int id)
        {
            return _begeleiders.SingleOrDefault(b => b.Id == id);
        }

        public void Update(Begeleider begeleider)
        {
            var teUpdatenBegeleider = FindById(begeleider.Id);
            if (teUpdatenBegeleider == null)
                return;
            teUpdatenBegeleider.Voornaam = begeleider.Voornaam;
            teUpdatenBegeleider.Familienaam = begeleider.Familienaam;
            teUpdatenBegeleider.Email = begeleider.Email;
            teUpdatenBegeleider.Gsm = begeleider.Gsm;
            teUpdatenBegeleider.Telefoon = begeleider.Telefoon;
            teUpdatenBegeleider.Postcode = begeleider.Postcode;
            teUpdatenBegeleider.Gemeente = begeleider.Gemeente;
            teUpdatenBegeleider.Straat = begeleider.Straat;

            if (teUpdatenBegeleider.Foto == null)
            {
                teUpdatenBegeleider.Foto = begeleider.Foto;
            }
            else if (begeleider.Foto != null)
            {
                teUpdatenBegeleider.Foto.FotoData = begeleider.Foto.FotoData;
                teUpdatenBegeleider.Foto.ContentType = begeleider.Foto.ContentType;
                teUpdatenBegeleider.Foto.Naam = begeleider.Foto.Naam;
            }
            SaveChanges();
        }

        public void Delete(Begeleider begeleider)
        {
            try
            {
                _begeleiders.Remove(begeleider);
                SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException.InnerException as MySqlException;
                if (sqlException != null && sqlException.Number == 1451)
                {
                    throw new ApplicationException(string.Format(Resources.ErrorDeleteBegeleider, begeleider.Naam));
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
                    message = eve.ValidationErrors.Aggregate(message, (current, ve) => current +
                        String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                }
                throw new ApplicationException("" + message);
            }
        }
    }
}