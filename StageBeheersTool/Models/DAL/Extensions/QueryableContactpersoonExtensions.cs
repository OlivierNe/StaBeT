using System.Linq;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.DAL.Extensions
{
    public static class QueryableContactpersoonExtensions
    {
        public static IQueryable<Contactpersoon> WithFilter(this IQueryable<Contactpersoon> contactpersonen,
           string bedrijf, string naam)
        {
            if (string.IsNullOrWhiteSpace(naam) && string.IsNullOrWhiteSpace(bedrijf))
            {
                return contactpersonen;
            }
            return contactpersonen.Where(cp => (string.IsNullOrEmpty(bedrijf) || cp.Bedrijf.Naam.ToLower().Contains(bedrijf.ToLower()))
                && (string.IsNullOrEmpty(naam) || (cp.Familienaam != null && cp.Familienaam.ToLower().Contains(naam.ToLower())) ||
                      (cp.Voornaam != null && cp.Voornaam.ToLower().Contains(naam.ToLower()))));
        }


    }
}