using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IBedrijfRepository
    {
        void Add(Bedrijf bedrijf);
        IQueryable<Bedrijf> FindAll();
        Bedrijf FindByEmail(string email);
        Bedrijf FindById(int id);
        void Update(Bedrijf bedrijf);
        void Delete(Bedrijf bedrijf);
        void SaveChanges();
    }
}
