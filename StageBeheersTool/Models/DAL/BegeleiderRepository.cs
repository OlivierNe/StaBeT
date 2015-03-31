using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL
{
    public class BegeleiderRepository : IBegeleiderRepository
    {
        private readonly DbSet<Begeleider> _begeleiders;
        private readonly StageToolDbContext _dbContext;

        public BegeleiderRepository(StageToolDbContext ctx)
        {
            this._dbContext = ctx;
            this._begeleiders = ctx.Begeleiders;
        }

        public void Add(Begeleider begeleider)
        {
            _begeleiders.Add(begeleider);
            SaveChanges();
        }

        public IQueryable<Begeleider> FindAll()
        {
            return _begeleiders.OrderBy(b => b.Naam);
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
            teUpdatenBegeleider.Gsmnummer = begeleider.Gsmnummer;
            teUpdatenBegeleider.Telefoonnummer = begeleider.Telefoonnummer;
            teUpdatenBegeleider.Postcode = begeleider.Postcode;
            teUpdatenBegeleider.Gemeente = begeleider.Gemeente;
            teUpdatenBegeleider.Straat = begeleider.Straat;
            teUpdatenBegeleider.Straatnummer = begeleider.Straatnummer;
            teUpdatenBegeleider.FotoUrl = begeleider.FotoUrl;
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