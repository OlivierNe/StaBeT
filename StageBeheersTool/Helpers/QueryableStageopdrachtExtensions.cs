using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using OudeGegevens.Models;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Helpers
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
            if (semester == null && aantalStudenten == null && specialisatie == null && bedrijf == null &&
                locatie == null && student == null)
            {
                return stageopdrachten;
            }
            return stageopdrachten.Where(so => (semester == null || ((so.Semester1 && semester == 1) || (so.Semester2 && semester == 2))) &&
                         (aantalStudenten == null || so.AantalStudenten == aantalStudenten) &&
                         (string.IsNullOrEmpty(specialisatie) ||
                          so.Specialisatie.ToLower().Contains(specialisatie.ToLower())) &&
                         (string.IsNullOrEmpty(bedrijf) || so.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower())) &&
                         (string.IsNullOrEmpty(locatie) || so.Gemeente.ToLower().Contains(locatie.ToLower())) &&
                         (string.IsNullOrEmpty(student) || so.Studenten.Any(s =>
                             (s.Familienaam != null && s.Familienaam.ToLower().Contains(student.ToLower())) ||
                                   (s.Voornaam != null && s.Voornaam.ToLower().Contains(student.ToLower())))));
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
                           (s.Familienaam != null && s.Familienaam.ToLower().Contains(student.ToLower())) ||
                           (s.Voornaam != null && s.Voornaam.ToLower().Contains(student.ToLower())))));
        }




    }
}