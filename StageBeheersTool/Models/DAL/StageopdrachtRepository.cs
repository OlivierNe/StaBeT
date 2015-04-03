using System.Linq.Expressions;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.Models.Domain;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using StageBeheersTool.Helpers;

namespace StageBeheersTool.Models.DAL
{
    public class StageopdrachtRepository : IStageopdrachtRepository
    {
        private readonly StageToolDbContext _dbContext;
        private readonly DbSet<Stageopdracht> _stageopdrachten;
        private readonly DbSet<StagebegeleidingAanvraag> _aanvragen;
        private readonly IUserService _userService;

        public StageopdrachtRepository(StageToolDbContext ctx, IUserService userService)
        {
            _dbContext = ctx;
            _userService = userService;
            _aanvragen = ctx.StageBegeleidAanvragen;
            _stageopdrachten = ctx.Stageopdrachten;
        }

        public Stageopdracht FindById(int id)
        {
            Stageopdracht stageopdracht;
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                stageopdracht = bedrijf.FindStageopdrachtById(id);
            }
            else if (CurrentUser.IsStudent())
            {
                stageopdracht = _stageopdrachten.Where(IsGeldig())
                    .IncludeAndOrder()
                    .SingleOrDefault(so => so.Id == id);
            }
            else // admin/begeleider
            {
                stageopdracht = _stageopdrachten.IncludeAndOrder()
                    .SingleOrDefault(so => so.Id == id);
            }
            return stageopdracht;
        }

        public IQueryable<Stageopdracht> FindAll()
        {
            IQueryable<Stageopdracht> stageopdrachten;
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                stageopdrachten = bedrijf.Stageopdrachten.OrderByDescending(so => so.Academiejaar).AsQueryable();
            }
            else if (CurrentUser.IsStudent())
            {
                stageopdrachten = _stageopdrachten.Where(IsGeldig()).IncludeAndOrder();
            }
            else // admin/begeleider
            {
                stageopdrachten = _stageopdrachten
                    .Where(IsGoedgekeurdEnVanHuidigAcademiejaar()).IncludeAndOrder();
            }
            return stageopdrachten;
        }

        public IQueryable<Stageopdracht> FindStageopdrachtVoorstellen()
        {
            return _stageopdrachten.Where(IsNietBeoordeeldEnVanHuidigAcademiejaar()).IncludeAndOrder();
        }

        public IQueryable<Stageopdracht> FindAllFromAcademiejaar(string academiejaar)
        {
            return _stageopdrachten.Where(so => so.Academiejaar == academiejaar).IncludeAndOrder();
        }

        public void Update(Stageopdracht stageopdracht)
        {
            var teUpdatenStageopdracht = FindById(stageopdracht.Id);
            if (teUpdatenStageopdracht == null)
                return;
            //if(!teUpdatenStageopdracht.IsInHuidigAcademiejaar())
            teUpdatenStageopdracht.Omschrijving = stageopdracht.Omschrijving;
            teUpdatenStageopdracht.Titel = stageopdracht.Titel;
            teUpdatenStageopdracht.Semester1 = stageopdracht.Semester1;
            teUpdatenStageopdracht.Semester2 = stageopdracht.Semester2;
            teUpdatenStageopdracht.Specialisatie = stageopdracht.Specialisatie;
            teUpdatenStageopdracht.Academiejaar = stageopdracht.Academiejaar;
            teUpdatenStageopdracht.AantalStudenten = stageopdracht.AantalStudenten;
            teUpdatenStageopdracht.Stagementor = stageopdracht.Stagementor;
            if (stageopdracht.Stagementor != null)
            {
                teUpdatenStageopdracht.StagementorEmail = stageopdracht.Stagementor.Email;
                teUpdatenStageopdracht.StagementorNaam = stageopdracht.Stagementor.Naam;
            }
            else
            {
                teUpdatenStageopdracht.StagementorEmail = null;
                teUpdatenStageopdracht.StagementorNaam = null;
            }
            teUpdatenStageopdracht.Contractondertekenaar = stageopdracht.Contractondertekenaar;
            if (stageopdracht.Contractondertekenaar != null)
            {
                teUpdatenStageopdracht.ContractondertekenaarEmail = stageopdracht.Contractondertekenaar.Email;
                teUpdatenStageopdracht.ContractondertekenaarNaam = stageopdracht.Contractondertekenaar.Naam;
            }
            else
            {
                teUpdatenStageopdracht.ContractondertekenaarEmail = null;
                teUpdatenStageopdracht.ContractondertekenaarNaam = null;
            }
            teUpdatenStageopdracht.Gemeente = stageopdracht.Gemeente;
            teUpdatenStageopdracht.Postcode = stageopdracht.Postcode;
            teUpdatenStageopdracht.Straat = stageopdracht.Straat;
            teUpdatenStageopdracht.Straatnummer = stageopdracht.Straatnummer;
            SaveChanges();
        }

        public void Delete(Stageopdracht stageopdracht)
        {
            _stageopdrachten.Remove(stageopdracht);
            SaveChanges();
        }

        public StagebegeleidingAanvraag FindAanvraagById(int id)
        {
            return _aanvragen
                .Include(sba => sba.Begeleider)
                .Include(sba => sba.Stage)
                .SingleOrDefault(a => a.Id == id);
        }

        public IQueryable<StagebegeleidingAanvraag> FindAllAanvragen()
        {
            return _aanvragen
                .Where(IsAanvraagInHuidigAcademiejaar())
                .OrderBy(sba => sba.Stage.Titel)
                .Include(sba => sba.Begeleider)
                .Include(sba => sba.Stage);
        }

        public IQueryable<StagebegeleidingAanvraag> FindAllAanvragenFrom(Begeleider begeleider)
        {
            return _aanvragen
                 .Where(sba => sba.Begeleider.Id == begeleider.Id)
                 .Where(IsAanvraagInHuidigAcademiejaar())
                 .OrderBy(sba => sba.Stage.Titel)
                 .Include(sba => sba.Begeleider)
                 .Include(sba => sba.Stage);
        }

        public IQueryable<Stageopdracht> FindStageopdrachtenVanBegeleider()
        {
            var begeleider = _userService.FindBegeleider();
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return _stageopdrachten.Where(so => so.Stagebegeleider.HogentEmail == begeleider.HogentEmail &&
                            so.Academiejaar == academiejaar).IncludeAndOrder();
        }

        public IQueryable<Stageopdracht> FindGeldigeBegeleiderStageopdrachten()
        {
            return _stageopdrachten
                .Where(KanBegleidWorden())
                .Where(so => so.Stagebegeleider == null)
                .IncludeAndOrder();
        }

        public void AddAanvraag(StagebegeleidingAanvraag aanvraag)
        {
            _aanvragen.Add(aanvraag);
            SaveChanges();
        }

        public void DeleteAanvraag(StagebegeleidingAanvraag aanvraag)
        {
            _aanvragen.Remove(aanvraag);
            SaveChanges();
        }

        public string[] FindAllAcademiejaren()
        {
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                return _stageopdrachten
                    .Where(b => b.Bedrijf.Email == bedrijf.Email)
                    .Select(so => so.Academiejaar).Distinct().OrderByDescending(s => s).ToArray();
            }
            return _stageopdrachten.Select(so => so.Academiejaar).Distinct().OrderByDescending(s => s).ToArray();
        }

        public IQueryable<Stageopdracht> FindAllVanAcademiejaar(string academiejaar)
        {
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                return _stageopdrachten.Where(b => b.Bedrijf.Email == bedrijf.Email)
                        .Where(so => so.Academiejaar == academiejaar)
                        .IncludeAndOrder();
            }
            return _stageopdrachten
                .Where(so => so.Academiejaar == academiejaar).IncludeAndOrder();
        }

        public string[] FindAllAcademiejarenVanBegeleider()
        {
            var begeleider = _userService.FindBegeleider();
            return _stageopdrachten
                .Where(so => so.Stagebegeleider.HogentEmail == begeleider.HogentEmail)
                .Select(so => so.Academiejaar).Distinct().OrderByDescending(s => s).ToArray();
        }

        public IQueryable<Stageopdracht> FindMijnStagesVanAcademiejaar(string academiejaar)
        {
            var begeleider = _userService.FindBegeleider();
            return _stageopdrachten
                .Where(so => so.Stagebegeleider.HogentEmail == begeleider.HogentEmail &&
                    (academiejaar == null || so.Academiejaar == academiejaar))
                    .IncludeAndOrder().OrderByDescending(so => so.Academiejaar);
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

        #region private helpers
        private Expression<Func<Stageopdracht, bool>> IsGeldig()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar &&
                         so.Status == StageopdrachtStatus.Goedgekeurd && so.Studenten.Count < so.AantalStudenten);
        }

        private Expression<Func<Stageopdracht, bool>> KanBegleidWorden()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar &&
                         so.Status == StageopdrachtStatus.Goedgekeurd && so.Studenten.Count >= so.AantalStudenten);
        }

        private Expression<Func<Stageopdracht, bool>> IsGoedgekeurdEnVanHuidigAcademiejaar()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar &&
                          so.Status == StageopdrachtStatus.Goedgekeurd);
        }

        private Expression<Func<Stageopdracht, bool>> IsNietBeoordeeldEnVanHuidigAcademiejaar()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar &&
                          so.Status == StageopdrachtStatus.NietBeoordeeld);
        }

        private Expression<Func<StagebegeleidingAanvraag, bool>> IsAanvraagInHuidigAcademiejaar()
        {
            var academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            return (aanvraag => aanvraag.Stage.Academiejaar == academiejaar);
        }
        #endregion
    }
}