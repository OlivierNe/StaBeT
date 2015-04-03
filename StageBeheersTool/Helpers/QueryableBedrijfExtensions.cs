using System.Linq;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Helpers
{
    public static class QueryableBedrijfExtensions
    {
        public static IQueryable<Bedrijf> WithFilter(this IQueryable<Bedrijf> bedrijven, string bedrijfsnaam)
        {
            if (string.IsNullOrWhiteSpace(bedrijfsnaam))
            {
                return bedrijven;
            }
            return bedrijven.Where(bedrijf => bedrijf.Naam != null && bedrijf.Naam.ToLower().Contains(bedrijfsnaam.ToLower()));
        }
    }
}