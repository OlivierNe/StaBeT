﻿using StageBeheersTool.Models.Domain;
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
            return bedrijven.Include(bedrijf => bedrijf.Stageopdrachten)
                .Include(bedrijf => bedrijf.Contactpersonen)
                .FirstOrDefault(bedrijf => bedrijf.Email == email);
        }

    }
}