using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
            stageopdrachten.Remove(stageopdracht);
            SaveChanges();
        }

        public IQueryable<Stageopdracht> FindAll()
        {
            return stageopdrachten.OrderBy(so => so.Titel)
                .Include(so => so.Bedrijf)
                .Include(so => so.Specialisatie);
        }

        public Stageopdracht FindById(int id)
        {
            return stageopdrachten.FirstOrDefault(so => so.Id == id);
        }

        public IQueryable<Stageopdracht> FindByFilter(int? semester, int? aantalStudenten, string soort, string bedrijf, string locatie)
        {
            return FindAll()
            .Where(so => (semester == null ? true : so.Semester == semester) &&
                (aantalStudenten == null ? true : so.AantalStudenten == aantalStudenten) &&
               (string.IsNullOrEmpty(soort) ? true : so.Specialisatie.Naam.ToLower().Contains(soort.ToLower())) &&
                  (string.IsNullOrEmpty(bedrijf) ? true : so.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower())) &&
                  (string.IsNullOrEmpty(locatie) ? true : so.Gemeente.ToLower().Contains(locatie.ToLower())));
        }

        public IQueryable<Stageopdracht> FindGeldigeStageopdrachten(int? semester, int? aantalStudenten, string soort, string bedrijf, string locatie)
        {
            return FindByFilter(semester, aantalStudenten, soort, bedrijf, locatie).AsEnumerable()
                .Where(so => so.IsGoedgekeurd() && !so.IsVolledigIngenomen() && so.IsInHuidigAcademiejaar()).AsQueryable();
        }

        /// <summary>
        /// geldig = goedgekeurd, niet volledig ingenomen en in huidig academiejaar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Stageopdracht FindGeldigeStageopdrachtById(int id)
        {
            return stageopdrachten.AsEnumerable().SingleOrDefault(so => so.Id == id &&
                so.IsGoedgekeurd() && !so.IsVolledigIngenomen() && so.IsInHuidigAcademiejaar());
        }

        public Stageopdracht FindGoedgekeurdeStageById(int id)
        {
            return stageopdrachten.SingleOrDefault(so => so.Id == id && so.Status == StageopdrachtStatus.Goedgekeurd);
        }

        public IQueryable<Stageopdracht> FindStageopdrachtenFrom(Begeleider begeleider)
        {
            return FindAll().Where(so => so.Stagebegeleider.Id == begeleider.Id);
        }

        public IQueryable<Stageopdracht> FindGoedgekeurdeStageopdrachten()
        {
            return stageopdrachten.Where(so => so.Status == StageopdrachtStatus.Goedgekeurd);
        }

        public IQueryable<Stageopdracht> FindGoedgekeurdeStageopdrachtenByFilter(int? semester, int? aantalStudenten,
            string soort, string bedrijf, string locatie, string student)
        {
            return FindByFilter(semester, aantalStudenten, soort, bedrijf, locatie)
                .Where(so => (so.Status == StageopdrachtStatus.Goedgekeurd)
                    && (student == null ? true : student == "" ? true :
                    so.Studenten.Any(st => student.ToLower().Contains(st.Voornaam.ToLower())
                        || student.ToLower().Contains(st.Familienaam.ToLower()))));
        }

        public void Update(Stageopdracht stageopdracht, Stageopdracht model)
        {
            stageopdracht.Omschrijving = model.Omschrijving;
            stageopdracht.Titel = model.Titel;
            stageopdracht.Semester = model.Semester;
            stageopdracht.Specialisatie = model.Specialisatie;
            stageopdracht.Academiejaar = model.Academiejaar;
            stageopdracht.AantalStudenten = model.AantalStudenten;
            stageopdracht.AantalToegewezenStudenten = model.AantalToegewezenStudenten;
            stageopdracht.Stagementor = model.Stagementor;
            stageopdracht.ContractOndertekenaar = model.ContractOndertekenaar;
            stageopdracht.Gemeente = model.Gemeente;
            stageopdracht.Postcode = model.Postcode;
            stageopdracht.Straat = model.Straat;
            stageopdracht.Straatnummer = model.Straatnummer;
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