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
            throw new NotImplementedException();
        }

        public IQueryable<Stageopdracht> FindAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Stageopdracht> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Stageopdracht> FindBy(string seachTerm)
        {
            throw new NotImplementedException();
        }
    }
}