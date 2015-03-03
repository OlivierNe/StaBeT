using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace StageBeheersTool.Models.DAL
{
    public class BedrijfRepository : IBedrijfRepository
    {
        private StageToolDbContext ctx;
        private DbSet<Bedrijf> bedrijven;
        private DbSet<Contactpersoon> contactpersonen;

        public BedrijfRepository(StageToolDbContext ctx)
        {
            this.ctx = ctx;
            this.bedrijven = ctx.Bedrijven;
            this.contactpersonen = ctx.Contactpersonen;
        }

        public void Add(Bedrijf bedrijf)
        {
            bedrijven.Add(bedrijf);
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

        public Bedrijf FindByEmail(string email)
        {
            return bedrijven
                .FirstOrDefault(bedrijf => bedrijf.Email == email);
        }

        public void DeleteContactpersoon(Contactpersoon contactpersoon)
        {
            contactpersonen.Remove(contactpersoon);
        }

        public void Update(Bedrijf bedrijf, Bedrijf newBedrijf)
        {
            bedrijf.Naam = newBedrijf.Naam;
            bedrijf.Gemeente = newBedrijf.Gemeente;
            bedrijf.Postcode = newBedrijf.Postcode;
            bedrijf.Straat = newBedrijf.Straat;
            bedrijf.Straatnummer = newBedrijf.Straatnummer;
            bedrijf.Bereikbaarheid = newBedrijf.Bereikbaarheid;
            bedrijf.BedrijfsActiviteiten = newBedrijf.BedrijfsActiviteiten;
            bedrijf.Telefoonnummer = newBedrijf.Telefoonnummer;
        }
    }
}