using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL
{
    public class SpecialisatieRepository : ISpecialisatieRepository
    {
        private DbSet<Specialisatie> specialisaties;

        public SpecialisatieRepository(StageToolDbContext ctx)
        {
            this.specialisaties = ctx.Specialisaties;
        }

        public IQueryable<Specialisatie> FindAll()
        {
            return specialisaties;
        }

        public Specialisatie FindBy(int id)
        {
            return specialisaties.SingleOrDefault(s => s.Id == id);
        }
    }
}