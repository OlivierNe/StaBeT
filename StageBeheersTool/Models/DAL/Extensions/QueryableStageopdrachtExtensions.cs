using System.Data.Entity;
using System.Linq;
using OudeGegevens.Models;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Extensions
{
    public static class QueryableStageopdrachtExtensions
    {
        public static IQueryable<Stageopdracht> IncludeAndOrder(this IQueryable<Stageopdracht> stageopdrachten)
        {
            return stageopdrachten.Include(so => so.Bedrijf)
                 .Include(so => so.Stagementor)
                 .Include(so => so.Contractondertekenaar)
                 .Include(so => so.Stagebegeleider)
                 .Include(so => so.Studenten)
                 .OrderBy(so => so.Titel);
        }


        public static IQueryable<Stageopdracht> WithFilter(this IQueryable<Stageopdracht> stageopdrachten,
           int? semester, int? aantalStudenten, string specialisatie, string bedrijf, string locatie, string student)
        {
            if (semester != null)
            {
                stageopdrachten = stageopdrachten.Where(so => (so.Semester1 && semester == 1) || (so.Semester2 && semester == 2));
            }
            if (aantalStudenten != null)
            {
                stageopdrachten = stageopdrachten.Where(so => so.AantalStudenten == aantalStudenten);
            }
            if (string.IsNullOrWhiteSpace(specialisatie) == false)
            {
                stageopdrachten = stageopdrachten.Where(so => so.Specialisatie != null && so.Specialisatie.ToLower().Contains(specialisatie.ToLower()));
            }
            if (string.IsNullOrWhiteSpace(bedrijf) == false)
            {
                stageopdrachten = stageopdrachten.Where(so => so.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower()));
            }
            if (string.IsNullOrWhiteSpace(locatie) == false)
            {
                stageopdrachten = stageopdrachten.Where(so => so.Gemeente.ToLower().Contains(locatie.ToLower())
                    || so.Bedrijf.Gemeente.ToLower().Contains(locatie.ToLower()));
            }
            if (string.IsNullOrWhiteSpace(student) == false)
            {
                stageopdrachten = stageopdrachten.Where(so => so.Studenten.Any(s =>
                    (s.Student.Familienaam != null && s.Student.Familienaam.ToLower().Contains(student.ToLower())) ||
                    (s.Student.Voornaam != null && s.Student.Voornaam.ToLower().Contains(student.ToLower()))));
            }
            return stageopdrachten;
        }

        public static IQueryable<Stageopdracht> WithFilter(this IQueryable<Stageopdracht> stageopdrachten,
            string bedrijf, string student)
        {
            if (student == null && bedrijf == null)
            {
                return stageopdrachten;
            }
            return stageopdrachten.Where(so => (string.IsNullOrEmpty(bedrijf) || so.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower())) &&
                       (string.IsNullOrEmpty(student) || so.Studenten.Any(s =>
                           (s.Student.Familienaam != null && s.Student.Familienaam.ToLower().Contains(student.ToLower())) ||
                           (s.Student.Voornaam != null && s.Student.Voornaam.ToLower().Contains(student.ToLower())))));
        }

        public static IQueryable<Stageopdracht> WithFilter(this IQueryable<Stageopdracht> stageopdrachten,
            int? begeleiderId, string academiejaar, StageopdrachtStatus? status)
        {
            if (begeleiderId != null)
            {
                stageopdrachten = stageopdrachten.Where(so => so.Stagebegeleider.Id == begeleiderId);
            }
            if (status != null)
            {
                stageopdrachten = stageopdrachten.Where(so => so.Status == status);
            }
            if (string.IsNullOrWhiteSpace(academiejaar) == false)
            {
                stageopdrachten = stageopdrachten.Where(so => so.Academiejaar == academiejaar);
            }

            return stageopdrachten;
        }
    }
}