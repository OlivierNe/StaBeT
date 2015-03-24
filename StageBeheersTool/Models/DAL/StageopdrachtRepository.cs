using System.Linq.Expressions;
using StageBeheersTool.Models.Domain;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace StageBeheersTool.Models.DAL
{
    public class StageopdrachtRepository : IStageopdrachtRepository
    {
        private readonly StageToolDbContext _ctx;
        private readonly DbSet<Stageopdracht> _stageopdrachten;
        private readonly DbSet<StageBegeleidAanvraag> _aanvragen;
        private readonly IUserService _userService;

        public StageopdrachtRepository(StageToolDbContext ctx, IUserService userService)
        {
            _ctx = ctx;
            _userService = userService;
            _aanvragen = ctx.StageBegeleidAanvragen;
            _stageopdrachten = ctx.Stageopdrachten;
        }

        public Stageopdracht FindById(int id)
        {
            Stageopdracht stageopdracht;
            if (_userService.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                stageopdracht = bedrijf.FindStageopdrachtById(id);
            }
            else if (_userService.IsStudent())
            {
                stageopdracht = _stageopdrachten.Where(IsGeldig())
                    .Include()
                    .SingleOrDefault(so => so.Id == id);
            }
            else // admin/begeleider
            {
                stageopdracht = _stageopdrachten.Include()
                    .SingleOrDefault(so => so.Id == id);
            }
            return stageopdracht;
        }

        public IQueryable<Stageopdracht> FindAll()
        {
            IQueryable<Stageopdracht> stageopdrachten;
            if (_userService.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                stageopdrachten = bedrijf.Stageopdrachten.AsQueryable();
            }
            else if (_userService.IsStudent())
            {
                stageopdrachten = _stageopdrachten.Where(IsGeldig());
            }
            else // admin/begeleider
            {
                stageopdrachten = _stageopdrachten
                    .Where(IsGoedgekeurdEnVanHuidigAcademiejaar());
            }
            return stageopdrachten.Include();
        }

        public IQueryable<Stageopdracht> FindStageopdrachtVoorstellen()
        {
            return _stageopdrachten.Where(IsNietBeoordeeldEnVanHuidigAcademiejaar()).OrderBy(so => so.Titel);
        }

        public IQueryable<Stageopdracht> FindAllWithFilter(int? semester, int? aantalStudenten,
            string specialisatie, string bedrijf, string locatie, string student)
        {
            if (semester == null && aantalStudenten == null && specialisatie == null && bedrijf == null && locatie == null && student == null)
            {
                return FindAll();
            }
            return FindAll().Where(Filter(semester, aantalStudenten, specialisatie, bedrijf, locatie, student)).OrderBy(so => so.Titel);
        }

        public IQueryable<Stageopdracht> FindAllFromAcademiejaar(string academiejaar)
        {
            return _stageopdrachten.Where(so => so.Academiejaar == academiejaar);
        }

        public IQueryable<Stageopdracht> FindGeldigeBegeleiderStageopdrachtenWithFilter(int? semester, int? aantalStudenten,
            string specialisatie, string bedrijf, string locatie, string student)
        {
            if (semester == null && aantalStudenten == null && specialisatie == null && bedrijf == null && locatie == null && student == null)
            {
                return _stageopdrachten.Where(IsGoedgekeurdEnVanHuidigAcademiejaar())
                        .Where(so => so.Stagebegeleider == null && so.Studenten.Count >= so.AantalStudenten)
                        .Include();
            }
            return _stageopdrachten
                .Where(IsGoedgekeurdEnVanHuidigAcademiejaar())
                .Where(so => so.Stagebegeleider == null)
                .Where(Filter(semester, aantalStudenten, specialisatie, bedrijf, locatie, student))
                .Include();
        }

        public void Update(Stageopdracht stageopdracht)
        {
            var teUpdatenStageopdracht = FindById(stageopdracht.Id);
            if (teUpdatenStageopdracht == null)
                return;
            teUpdatenStageopdracht.Omschrijving = stageopdracht.Omschrijving;
            teUpdatenStageopdracht.Titel = stageopdracht.Titel;
            teUpdatenStageopdracht.Semester1 = stageopdracht.Semester1;
            teUpdatenStageopdracht.Semester2 = stageopdracht.Semester2;
            teUpdatenStageopdracht.Specialisatie = stageopdracht.Specialisatie;
            teUpdatenStageopdracht.Academiejaar = stageopdracht.Academiejaar;
            teUpdatenStageopdracht.AantalStudenten = stageopdracht.AantalStudenten;
            teUpdatenStageopdracht.Stagementor = stageopdracht.Stagementor;
            teUpdatenStageopdracht.Contractondertekenaar = stageopdracht.Contractondertekenaar;
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

        public StageBegeleidAanvraag FindAanvraagById(int id)
        {
            return _aanvragen
                .Include(sba => sba.Begeleider)
                .Include(sba => sba.Stageopdracht)
                .SingleOrDefault(a => a.Id == id);
        }

        public IQueryable<StageBegeleidAanvraag> FindAllAanvragen()
        {
            return _aanvragen
                .Where(IsAanvraagInHuidigAcademiejaar())
                .OrderBy(sba => sba.Begeleider.Familienaam)
                .Include(sba => sba.Begeleider)
                .Include(sba => sba.Stageopdracht);
        }

        public IQueryable<StageBegeleidAanvraag> FindAllAanvragenFrom(Begeleider begeleider)
        {
            return _aanvragen
                 .Where(sba => sba.Begeleider.Id == begeleider.Id)
                 .Where(IsAanvraagInHuidigAcademiejaar())
                 .OrderBy(sba => sba.Begeleider.Familienaam)
                 .Include(sba => sba.Begeleider)
                 .Include(sba => sba.Stageopdracht);
        }

        public IQueryable<Stageopdracht> FindStageopdrachtenVanBegeleider()
        {
            var begeleider = _userService.FindBegeleider();
            var academiejaar = Helpers.HuidigAcademiejaar();
            return _stageopdrachten.Where(so => so.Stagebegeleider.HogentEmail == begeleider.HogentEmail &&
                            so.Academiejaar == academiejaar).Include();
        }

        public void AddAanvraag(StageBegeleidAanvraag aanvraag)
        {
            _aanvragen.Add(aanvraag);
            SaveChanges();
        }

        public void DeleteAanvraag(StageBegeleidAanvraag aanvraag)
        {
            _aanvragen.Remove(aanvraag);
            SaveChanges();
        }

        public string[] FindAllAcademiejaren()
        {
            return _stageopdrachten.Select(so => so.Academiejaar).Distinct().OrderBy(s => s).ToArray();
        }

        public IQueryable<Stageopdracht> FindAllVanAcademiejaar(string academiejaar, string student, string bedrijf)
        {
            return _stageopdrachten
                .Where(so => so.Academiejaar == academiejaar)
                .Where(StudentEnBedrijfFilter(student, bedrijf)).Include();
        }

        public string[] FindAllAcademiejarenVanBegeleider()
        {
            var begeleider = _userService.FindBegeleider();
            return _stageopdrachten
                .Where(so => so.Stagebegeleider.HogentEmail == begeleider.HogentEmail)
                .Select(so => so.Academiejaar).Distinct().OrderBy(s => s).ToArray();
        }

        public IQueryable<Stageopdracht> FindMijnStagesVanAcademiejaar(string academiejaar, string student, string bedrijf)
        {
            var begeleider = _userService.FindBegeleider();
            return _stageopdrachten
                .Where(so => so.Stagebegeleider.HogentEmail == begeleider.HogentEmail &&
                so.Academiejaar == academiejaar)
                .Where(StudentEnBedrijfFilter(student, bedrijf)).Include();
        }

        public void SaveChanges()
        {
            try
            {
                _ctx.SaveChanges();
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
        private Expression<Func<Stageopdracht, bool>> Filter(int? semester, int? aantalStudenten,
            string specialisatie, string bedrijf, string locatie, string student)
        {
            return so => (semester == null || ((so.Semester1 && semester == 1) || (so.Semester2 && semester == 2))) &&
                         (aantalStudenten == null || so.AantalStudenten == aantalStudenten) &&
                         (string.IsNullOrEmpty(specialisatie) ||
                          so.Specialisatie.ToLower().Contains(specialisatie.ToLower())) &&
                         (string.IsNullOrEmpty(bedrijf) || so.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower())) &&
                         (string.IsNullOrEmpty(locatie) || so.Gemeente.ToLower().Contains(locatie.ToLower())) &&
                         (string.IsNullOrEmpty(student) || so.Studenten.Any(s =>
                             (s.Familienaam != null && s.Familienaam.ToLower().Contains(student.ToLower())) ||
                                   (s.Voornaam != null && s.Voornaam.ToLower().Contains(student.ToLower()))));
        }

        private Expression<Func<Stageopdracht, bool>> StudentEnBedrijfFilter(string student, string bedrijf)
        {
            return so => (string.IsNullOrEmpty(bedrijf) || so.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower())) &&
                         (string.IsNullOrEmpty(student) || so.Studenten.Any(s =>
                             (s.Familienaam != null && s.Familienaam.ToLower().Contains(student.ToLower())) ||
                                   (s.Voornaam != null && s.Voornaam.ToLower().Contains(student.ToLower()))));
        }

        private Expression<Func<Stageopdracht, bool>> IsGeldig()
        {
            var academiejaar = Helpers.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar &&
                         so.Status == StageopdrachtStatus.Goedgekeurd && so.Studenten.Count != so.AantalStudenten);
        }

        private Expression<Func<Stageopdracht, bool>> IsGoedgekeurdEnVanHuidigAcademiejaar()
        {
            var academiejaar = Helpers.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar &&
                          so.Status == StageopdrachtStatus.Goedgekeurd);
        }

        private Expression<Func<Stageopdracht, bool>> IsNietBeoordeeldEnVanHuidigAcademiejaar()
        {
            var academiejaar = Helpers.HuidigAcademiejaar();
            return (so => so.Academiejaar == academiejaar &&
                          so.Status == StageopdrachtStatus.NietBeoordeeld);
        }

        private Expression<Func<StageBegeleidAanvraag, bool>> IsAanvraagInHuidigAcademiejaar()
        {
            var academiejaar = Helpers.HuidigAcademiejaar();
            return (aanvraag => aanvraag.Stageopdracht.Academiejaar == academiejaar);
        }

        #endregion

    }

    public static class QueryableStageopdrachtExtensions
    {
        public static IQueryable<Stageopdracht> Include(this IQueryable<Stageopdracht> stageopdrachten)
        {
            return stageopdrachten.Include(so => so.Bedrijf)
                 .Include(so => so.Stagementor)
                 .Include(so => so.Contractondertekenaar)
                 .Include(so => so.Stagebegeleider)
                 .Include(so => so.Studenten)
                 .OrderBy(so => so.Titel);
        }
    }
}