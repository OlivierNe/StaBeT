using System.Linq;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Helpers
{
    public static class QueryableStudentExtensions
    {
        public static IQueryable<Student> WithFilter(this IQueryable<Student> studenten, string familienaam, string voornaam)
        {
            if (string.IsNullOrWhiteSpace(familienaam) && string.IsNullOrWhiteSpace(voornaam))
            {
                return studenten;
            }
            if (!string.IsNullOrWhiteSpace(familienaam) && string.IsNullOrWhiteSpace(voornaam))
            {
                return studenten.Where(student => student.Familienaam.ToLower().Contains(familienaam.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(voornaam) && string.IsNullOrWhiteSpace(familienaam))
            {
                return studenten.Where(student => student.Voornaam.ToLower().Contains(voornaam.ToLower()));
            }
            return studenten.Where(student => student.Familienaam.ToLower().Contains(familienaam.ToLower())).Where(student => student.Voornaam.ToLower().Contains(voornaam.ToLower()));
        }


    }
}