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
        private DbSet<Begeleider> begeleiders;
        private StageToolDbContext ctx;

        public BegeleiderRepository(StageToolDbContext ctx)
        {
            this.ctx = ctx;
            this.begeleiders = ctx.Begeleiders;
        }

        public void Add(Begeleider begeleider)
        {
            begeleiders.Add(begeleider);
            SaveChanges();
        }

        public Begeleider FindByEmail(string hoGentEmail)
        {
            return begeleiders.SingleOrDefault(b => b.HogentEmail == hoGentEmail);
        }

        public Begeleider FindById(int id)
        {
            return begeleiders.SingleOrDefault(b => b.Id == id);
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
                ctx.SaveChanges();
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