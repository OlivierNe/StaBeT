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

        public IQueryable<Stageopdracht> FindByFilter(int? semester, string soort, string bedrijf, string locatie)
        {
            return FindAll()
            .Where(so => (semester == null ? true : so.Semester == semester) &&
               (string.IsNullOrEmpty(soort) ? true :
                  so.Specialisatie.Naam.ToLower().Contains(soort.ToLower())) &&
                  (string.IsNullOrEmpty(bedrijf) ? true : so.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower())) &&
                  (string.IsNullOrEmpty(locatie) ? true : so.Bedrijf.Gemeente.ToLower().Contains(locatie.ToLower())));
        }

        public IQueryable<Stageopdracht> FindGeldigeStageopdrachten(int? semester, string soort, string bedrijf, string locatie)
        {
            return FindByFilter(semester, soort, bedrijf, locatie).AsEnumerable()
                .Where(so => so.IsGoedgekeurd() && so.isVolledigIngenomen() && so.IsInHuidigAcademiejaar()).AsQueryable();
        }


    }
}