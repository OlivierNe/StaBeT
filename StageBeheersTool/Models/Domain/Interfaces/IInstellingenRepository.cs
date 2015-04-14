using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IInstellingenRepository
    {
        IQueryable<Instelling> FindAll();
        Instelling Find(string key);
        void AddOrUpdate(Instelling instelling);
        void SaveChanges();
    }
}
