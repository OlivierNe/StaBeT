using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;

namespace StageBeheersTool.Models.DAL
{
    public class AcademiejaarRepository : IAcademiejaarRepository
    {
        private StageToolDbContext ctx;
        private DbSet<AcademiejaarInstellingen> academiejaren;

        public AcademiejaarRepository(StageToolDbContext ctx)
        {
            this.ctx = ctx;
            academiejaren = ctx.AcademiejarenInstellingen;
        }

        public void Add(AcademiejaarInstellingen academiejaar)
        {
            academiejaren.AddOrUpdate(academiejaar);
            SaveChanges();
        }

        public AcademiejaarInstellingen FindByHuidigAcademiejaar()
        {
            string huidigAcademiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return academiejaren.SingleOrDefault(aj => aj.Academiejaar.Equals(huidigAcademiejaar));
        }

        public AcademiejaarInstellingen FindByAcademiejaar(string academiejaar)
        {
            return academiejaren.Find(academiejaar);
        }

        public IQueryable<AcademiejaarInstellingen> FindAll()
        {
            return academiejaren;
        }

        public void Update(AcademiejaarInstellingen academiejaar)
        {
            ctx.Entry(academiejaar).State = EntityState.Modified;
            SaveChanges();
        }

        public void Delete(AcademiejaarInstellingen academiejaar)
        {
            academiejaren.Remove(academiejaar);
            SaveChanges();
        }

        public void SaveChanges()
        {
            ctx.SaveChanges();
        }

    }
}