using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL
{
    public class ContactpersoonRepository : IContactpersoonRepository
    {
        private DbSet<Contactpersoon> contactpersonen;
        private StageToolDbContext ctx;

        public ContactpersoonRepository(StageToolDbContext ctx)
        {
            this.ctx = ctx;
            contactpersonen = ctx.Contactpersonen;
        }

        public void Delete(Contactpersoon contactpersoon)
        {
            contactpersonen.Remove(contactpersoon);
        }

        public Contactpersoon FindById(int id)
        {
            return contactpersonen.FirstOrDefault(cp => cp.Id == id);
        }

        public IQueryable<Contactpersoon> Contactpersonen()
        {
            return contactpersonen;
        }

        public void SaveChanges()
        {
            ctx.SaveChanges();
        }
    }
}