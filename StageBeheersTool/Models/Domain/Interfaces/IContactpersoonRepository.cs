using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IContactpersoonRepository
    {
        void Delete(Contactpersoon contactpersoon);
        IQueryable<Contactpersoon> FindAll();
        IQueryable<Contactpersoon> FindAllVanBedrijf(int bedrijfId);
        Contactpersoon FindById(int id);
        void Update(Contactpersoon contactpersoon);
        void SaveChanges();
    }
}
