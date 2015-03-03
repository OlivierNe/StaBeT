using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL
{
    public class KeuzepakketRepository : IKeuzepakketRepository
    {

        private DbSet<Keuzepakket> keuzepakketten;

        public KeuzepakketRepository(StageToolDbContext ctx)
        {
            this.keuzepakketten = ctx.Keuzepakketten;
        }

        public IQueryable<Keuzepakket> FindAll()
        {
            return keuzepakketten;
        }

        public Keuzepakket FindBy(int id)
        {
            return keuzepakketten.FirstOrDefault(k => k.Id == id);
        }
    }
}