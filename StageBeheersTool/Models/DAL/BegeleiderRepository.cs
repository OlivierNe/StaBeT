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
        private DbSet<StageBegeleidAanvraag> aanvragen;
        private StageToolDbContext ctx;

        public BegeleiderRepository(StageToolDbContext ctx)
        {
            this.ctx = ctx;
            this.begeleiders = ctx.Begeleiders;
            this.aanvragen = ctx.StageBegeleidAanvragen;
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
        
        public void Update(Begeleider begeleider, Begeleider model)
        {
            begeleider.Voornaam = model.Voornaam;
            begeleider.Familienaam = model.Familienaam;
            begeleider.Email = model.Email;
            begeleider.Gsmnummer = model.Gsmnummer;
            begeleider.Telefoonnummer = model.Telefoonnummer;
            begeleider.Postcode = model.Postcode;
            begeleider.Gemeente = model.Gemeente;
            begeleider.Straat = model.Straat;
            begeleider.Straatnummer = model.Straatnummer;
            begeleider.FotoUrl = model.FotoUrl;
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