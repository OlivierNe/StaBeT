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

        public void Delete(Stageopdracht stageopdracht)
        {
            _stageopdrachten.Remove(stageopdracht);
            SaveChanges();
        }

        public Stageopdracht FindById(int id)
        {
            return _stageopdrachten
                .Include(so => so.Specialisatie)
                .Include(so => so.Bedrijf)
                .Include(so => so.Stagementor)
                .Include(so => so.Contractondertekenaar)
                .Include(so => so.Stagebegeleider)
                .Include(so => so.Studenten)
                .SingleOrDefault(so => so.Id == id);
        }

        public IQueryable<Stageopdracht> FindAll()
        {
            if (_userService.IsBedrijf())
            {
                return _userService.FindBedrijf().Stageopdrachten.AsQueryable()
                    .OrderBy(so => so.Titel)
                    .Include(so => so.Bedrijf)
                    .Include(so => so.Specialisatie);
            }
            if (_userService.IsStudent())
            {
                return _stageopdrachten.Where(_goedGekeurd)
                    .OrderBy(so => so.Titel)
                    .Include(so => so.Bedrijf)
                    .Include(so => so.Specialisatie);
            }
            return _stageopdrachten.OrderBy(so => so.Titel)
                .Include(so => so.Bedrijf)
                .Include(so => so.Specialisatie);
        }

        public IQueryable<Stageopdracht> FindAllByFilter(int? semester, int? aantalStudenten, int? specialisatieId,
            string bedrijf, string locatie, string student)
        {
            if (semester == null && aantalStudenten == null && specialisatieId == null && bedrijf == null && locatie == null && student == null)
            {
                return FindAll();
            }
            if (_userService.IsBedrijf())
            {
                return _userService.FindBedrijf().Stageopdrachten.AsQueryable()
                    .Where(Filter(semester, aantalStudenten, specialisatieId, null, locatie, student))
                    .OrderBy(so => so.Titel);
            }
            if (_userService.IsStudent())
            {
                return FindGeldigeStageopdrachten(semester, aantalStudenten, specialisatieId,
                       bedrijf, locatie);
            }
            return FindByFilter(semester, aantalStudenten, specialisatieId,
                bedrijf, locatie);

        }

        public IQueryable<Stageopdracht> FindByFilter(int? semester, int? aantalStudenten, int? specialisatieId, string bedrijf, string locatie)
        {
            if (semester == null && aantalStudenten == null && specialisatieId == null && bedrijf == null && locatie == null)
            {
                return FindAll();
            }
            return FindAll().Where(Filter(semester, aantalStudenten, specialisatieId, bedrijf, locatie, null));
        }

        public IQueryable<Stageopdracht> FindGeldigeStageopdrachten(int? semester, int? aantalStudenten, int? specialisatieId, string bedrijf, string locatie)
        {
            return FindByFilter(semester, aantalStudenten, specialisatieId, bedrijf, locatie).AsEnumerable()
                .Where(_geldig).AsQueryable();
        }

        /// <summary>
        /// geldig = goedgekeurd, niet volledig ingenomen en in huidig academiejaar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Stageopdracht FindGeldigeStageopdrachtById(int id)
        {
            return _stageopdrachten.AsEnumerable().SingleOrDefault(so => so.Id == id &&
                so.IsGoedgekeurd() && !so.IsVolledigIngenomen() && so.IsInHuidigAcademiejaar());
        }

        public Stageopdracht FindGoedgekeurdeStageById(int id)
        {
            return _stageopdrachten.SingleOrDefault(so => so.Id == id && so.Status == StageopdrachtStatus.Goedgekeurd);
        }

        public IQueryable<Stageopdracht> FindStageopdrachtenFrom(Begeleider begeleider)
        {
            return FindAll().Where(so => so.Stagebegeleider.Id == begeleider.Id);
        }

        public IQueryable<Stageopdracht> FindGoedgekeurdeStageopdrachten()
        {
            return _stageopdrachten.Where(so => so.Status == StageopdrachtStatus.Goedgekeurd);
        }

        public IQueryable<Stageopdracht> FindGoedgekeurdeStageopdrachtenByFilter(int? semester, int? aantalStudenten,
            int? specialisatieId, string bedrijf, string locatie, string student)
        {
            return FindByFilter(semester, aantalStudenten, specialisatieId, bedrijf, locatie)
                .Where(so => (so.Status == StageopdrachtStatus.Goedgekeurd)
                    && (student == null || (student == "" || so.Studenten.Any(st => student.ToLower().Contains(st.Voornaam.ToLower())
                        || student.ToLower().Contains(st.Familienaam.ToLower()))))).AsEnumerable()
                        .Where(so => so.IsInHuidigAcademiejaar()).AsQueryable();
        }
        public IQueryable<Stageopdracht> FindStageopdrachtVoorstellen()
        {
            return FindAll().Where(so => so.Status == StageopdrachtStatus.NietBeoordeeld);
        }

        public void Update(Stageopdracht stageopdracht)
        {
            var teUpdatenStageopdracht = _stageopdrachten.SingleOrDefault(so => so.Id == stageopdracht.Id);
            if (teUpdatenStageopdracht == null)
                return;
            teUpdatenStageopdracht.Omschrijving = stageopdracht.Omschrijving;
            teUpdatenStageopdracht.Titel = stageopdracht.Titel;
            teUpdatenStageopdracht.Semester1 = stageopdracht.Semester1;
            teUpdatenStageopdracht.Semester2 = stageopdracht.Semester2;
            teUpdatenStageopdracht.Specialisatie = stageopdracht.Specialisatie;
            teUpdatenStageopdracht.Academiejaar = stageopdracht.Academiejaar;
            teUpdatenStageopdracht.AantalStudenten = stageopdracht.AantalStudenten;
            teUpdatenStageopdracht.AantalToegewezenStudenten = stageopdracht.AantalToegewezenStudenten;
            teUpdatenStageopdracht.Stagementor = stageopdracht.Stagementor;
            teUpdatenStageopdracht.Contractondertekenaar = stageopdracht.Contractondertekenaar;
            teUpdatenStageopdracht.Gemeente = stageopdracht.Gemeente;
            teUpdatenStageopdracht.Postcode = stageopdracht.Postcode;
            teUpdatenStageopdracht.Straat = stageopdracht.Straat;
            teUpdatenStageopdracht.Straatnummer = stageopdracht.Straatnummer;
            SaveChanges();
        }

        public StageBegeleidAanvraag FindAanvraagById(int id)
        {
            return _aanvragen.Include(sba => sba.Begeleider).Include(sba => sba.Stageopdracht).SingleOrDefault(a => a.Id == id);
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

        public IQueryable<StageBegeleidAanvraag> FindAllAanvragen()
        {
            return _aanvragen.OrderBy(sba => sba.Begeleider.Familienaam).Include(sba => sba.Begeleider).Include(sba => sba.Stageopdracht);
        }

        public IQueryable<StageBegeleidAanvraag> FindAllAanvragenFrom(Begeleider begeleider)
        {
            return FindAllAanvragen().Where(sba => sba.Begeleider.Id == begeleider.Id);
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
            int? specialisatieId, string bedrijf, string locatie, string student)
        {
            return so => (semester == null || ((so.Semester1 && semester == 1) || (so.Semester2 && semester == 2))) &&
                      (aantalStudenten == null || so.AantalStudenten == aantalStudenten) &&
                      ((specialisatieId == null || so.Specialisatie == null) || (specialisatieId == so.Specialisatie.Id)) &&
                      (string.IsNullOrEmpty(bedrijf) || so.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower())) &&
                      (string.IsNullOrEmpty(locatie) || so.Gemeente.ToLower().Contains(locatie.ToLower()));
        }

        private readonly Expression<Func<Stageopdracht, bool>> _goedGekeurd = so => so.Status == StageopdrachtStatus.Goedgekeurd;

        private readonly Func<Stageopdracht, bool> _geldig =
            so => so.IsGoedgekeurd() && !so.IsVolledigIngenomen() && so.IsInHuidigAcademiejaar();

        #endregion

    }
}