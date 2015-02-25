using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data;

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
            ctx.SaveChanges();
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

    }
}