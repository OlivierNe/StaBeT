using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL
{
    public class StageopdrachtRepository : IStageopdrachtRepository
    {
        private StageToolDbContext ctx;
        private DbSet<Stageopdracht> stageopdrachten;

        public StageopdrachtRepository(StageToolDbContext ctx)
        {
            this.ctx = ctx;
            this.stageopdrachten = ctx.Stageopdrachten;
        }

        public void Delete(Stageopdracht stageopdracht)
        {
            stageopdrachten.Remove(stageopdracht);
        }

        public IQueryable<Stageopdracht> FindAll()
        {
            return stageopdrachten.OrderByDescending(so => so.Id);
        }

        public Stageopdracht FindById(int id)
        {
            return stageopdrachten.FirstOrDefault(so => so.Id == id);
        }

        public IQueryable<Stageopdracht> FindBy(string seachTerm)
        {
            throw new NotImplementedException();
        }
    }
}