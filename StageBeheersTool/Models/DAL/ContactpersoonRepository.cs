using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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

        public Contactpersoon FindById(int id)
        {
            return contactpersonen.SingleOrDefault(cp => cp.Id == id);
        }

        public IQueryable<Contactpersoon> Contactpersonen()
        {
            return contactpersonen;
        }

        public void Delete(Contactpersoon contactpersoon)
        {
            contactpersonen.Remove(contactpersoon);
            SaveChanges();
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
    }
}