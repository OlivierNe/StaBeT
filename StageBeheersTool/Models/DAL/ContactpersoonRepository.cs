using StageBeheersTool.Models.Domain;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace StageBeheersTool.Models.DAL
{
    public class ContactpersoonRepository : IContactpersoonRepository
    {
        private readonly DbSet<Contactpersoon> _contactpersonen;
        private readonly StageToolDbContext _dbContext;

        public ContactpersoonRepository(StageToolDbContext dbContext)
        {
            _dbContext = dbContext;
            _contactpersonen = dbContext.Contactpersonen;
        }

        public IQueryable<Contactpersoon> FindAll()
        {
            return _contactpersonen.OrderBy(cp => cp.Id).Include(cp => cp.Bedrijf);
        }

        public IQueryable<Contactpersoon> FindAllVanBedrijf(int bedrijfId)
        {
            return _contactpersonen.Where(cp => cp.Bedrijf.Id == bedrijfId)
                .OrderBy(cp => cp.Id).Include(cp => cp.Bedrijf);
        }

        public Contactpersoon FindById(int id)
        {
            return _contactpersonen.SingleOrDefault(cp => cp.Id == id);
        }

        public void Delete(Contactpersoon contactpersoon)
        {
            _contactpersonen.Remove(contactpersoon);
            SaveChanges();
        }

        public void Update(Contactpersoon contactpersoon)
        {
            var teUpdatenPersoon = FindById(contactpersoon.Id);
            if (teUpdatenPersoon == null)
                return;
            teUpdatenPersoon.Voornaam = contactpersoon.Voornaam;
            teUpdatenPersoon.Familienaam = contactpersoon.Familienaam;
            teUpdatenPersoon.Gsm = contactpersoon.Gsm;
            teUpdatenPersoon.Telefoon = contactpersoon.Telefoon;
            teUpdatenPersoon.IsStagementor = contactpersoon.IsStagementor;
            teUpdatenPersoon.IsContractondertekenaar = contactpersoon.IsContractondertekenaar;
            teUpdatenPersoon.Aanspreektitel = contactpersoon.Aanspreektitel;
            teUpdatenPersoon.Bedrijfsfunctie = contactpersoon.Bedrijfsfunctie;
            teUpdatenPersoon.Email = contactpersoon.Email;

            SaveChanges();
        }

        public void SaveChanges()
        {
            try
            {
                _dbContext.SaveChanges();
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