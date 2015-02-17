using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL
{
    public class BedrijfRepository : IBedrijfRepository
    {
        private StageToolDbContext ctx;
        private DbSet<Bedrijf> bedrijven;

        public BedrijfRepository(StageToolDbContext ctx)
        {
            this.ctx = ctx;
            bedrijven = ctx.Bedrijven;
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
            return bedrijven.FirstOrDefault(bedrijf => bedrijf.Email == email);
        }
    }
}