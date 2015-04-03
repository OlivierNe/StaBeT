using System.Linq;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Helpers
{
    public static class QueryableBegeleiderExtensions
    {
        public static IQueryable<Begeleider> WithFilter(this IQueryable<Begeleider> begeleiders, string familienaam, string voornaam)
        {
            if (string.IsNullOrWhiteSpace(familienaam) && string.IsNullOrWhiteSpace(voornaam))
            {
                return begeleiders;
            }
            if (!string.IsNullOrWhiteSpace(familienaam) && string.IsNullOrWhiteSpace(voornaam))
            {
                return begeleiders.Where(student => student.Familienaam.ToLower().Contains(familienaam.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(voornaam) && string.IsNullOrWhiteSpace(familienaam))
            {
                return begeleiders.Where(student => student.Voornaam.ToLower().Contains(voornaam.ToLower()));
            }
            return begeleiders.Where(student => student.Familienaam.ToLower().Contains(familienaam.ToLower())).Where(student => student.Voornaam.ToLower().Contains(voornaam.ToLower()));
        }
    }
}