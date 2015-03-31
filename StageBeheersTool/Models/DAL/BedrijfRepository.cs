using StageBeheersTool.Models.Domain;
using System;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Validation;

namespace StageBeheersTool.Models.DAL
{
    public class BedrijfRepository : IBedrijfRepository
    {
        private readonly StageToolDbContext ctx;
        private readonly DbSet<Bedrijf> _bedrijven;
        private readonly IUserService _userService;

        public BedrijfRepository(StageToolDbContext ctx, IUserService userService)
        {
            this.ctx = ctx;
            this._bedrijven = ctx.Bedrijven;
            this._userService = userService;
        }

        public void Add(Bedrijf bedrijf)
        {
            _bedrijven.Add(bedrijf);
            SaveChanges();
        }

        public Bedrijf FindByEmail(string email)
        {
            if (_userService.IsBedrijf())
            {
                return _userService.FindBedrijf();
            }
            return _bedrijven
                .SingleOrDefault(bedrijf => bedrijf.Email == email);
        }

        public Bedrijf FindById(int? id)
        {
            if (_userService.IsBedrijf())
            {
                return _userService.FindBedrijf();
            }
            if (id == null)
            {
                return null;
            }
            return _bedrijven.SingleOrDefault(bedrijf => bedrijf.Id == id);
        }

        public void Update(Bedrijf bedrijf)
        {
            var teUpdatenBedrijf = FindById(bedrijf.Id);
            if (teUpdatenBedrijf == null)
                return;
            teUpdatenBedrijf.Naam = bedrijf.Naam;
            teUpdatenBedrijf.Gemeente = bedrijf.Gemeente;
            teUpdatenBedrijf.Postcode = bedrijf.Postcode;
            teUpdatenBedrijf.Straat = bedrijf.Straat;
            teUpdatenBedrijf.Straatnummer = bedrijf.Straatnummer;
            teUpdatenBedrijf.Bereikbaarheid = bedrijf.Bereikbaarheid;
            teUpdatenBedrijf.Bedrijfsactiviteiten = bedrijf.Bedrijfsactiviteiten;
            teUpdatenBedrijf.Telefoonnummer = bedrijf.Telefoonnummer;
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

        public IQueryable<Bedrijf> FindAll()
        {
            return _bedrijven.OrderBy(b => b.Naam);
        }
    }
}