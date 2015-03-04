using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                .Where(so => so.IsGoedgekeurd() && so.IsVolledigIngenomen() && so.IsInHuidigAcademiejaar()).AsQueryable();
        }

        public Stageopdracht FindGeldigeStageopdrachtById(int id)
        {
            return stageopdrachten.AsEnumerable().FirstOrDefault(so => so.IsGoedgekeurd() && so.IsVolledigIngenomen() && so.IsInHuidigAcademiejaar() && so.Id == id);
        }

        public IQueryable<Stageopdracht> FindStageopdrachtenFrom(Begeleider begeleider)
        {
            return FindAll().Where(so => so.Stagebegeleider.Id == begeleider.Id);
        }


        public void Update(Stageopdracht stageopdracht, Stageopdracht teUpdatenOpdracht)
        {
            //db.Users.Attach(updatedUser);
            //var entry = db.Entry(updatedUser);
            //entry.Property(e => e.Email).IsModified = true;
            //// other changed properties
            //db.SaveChanges();

            //ctx.Stageopdrachten.Attach(stageopdracht);
            //var entry = ctx.Entry(stageopdrachten);
            //ctx.Stageopdrachten.Attach(stageopdracht);
            //ctx.Entry(stageopdracht).State = EntityState.Modified;
            //ctx.SaveChanges();
            teUpdatenOpdracht.Omschrijving = stageopdracht.Omschrijving;
            teUpdatenOpdracht.Titel = stageopdracht.Titel;
            teUpdatenOpdracht.Semester = stageopdracht.Semester;
            teUpdatenOpdracht.Specialisatie = stageopdracht.Specialisatie;
            teUpdatenOpdracht.Academiejaar = stageopdracht.Academiejaar;
            teUpdatenOpdracht.AantalStudenten = stageopdracht.AantalStudenten;
            teUpdatenOpdracht.AantalToegewezenStudenten = stageopdracht.AantalToegewezenStudenten;
            teUpdatenOpdracht.Stagementor = stageopdracht.Stagementor;
            teUpdatenOpdracht.ContractOndertekenaar = stageopdracht.ContractOndertekenaar;
            teUpdatenOpdracht.Gemeente = stageopdracht.Gemeente;
            teUpdatenOpdracht.Postcode = stageopdracht.Postcode;
            teUpdatenOpdracht.Straat = stageopdracht.Straat;
            teUpdatenOpdracht.Straatnummer = stageopdracht.Straatnummer;

        }
    }
}