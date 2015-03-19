using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IContactpersoonRepository
    {
        void Delete(Contactpersoon contactpersoon);
        Contactpersoon FindById(int id);
        IQueryable<Contactpersoon> Contactpersonen();
        void SaveChanges();
    }
}
