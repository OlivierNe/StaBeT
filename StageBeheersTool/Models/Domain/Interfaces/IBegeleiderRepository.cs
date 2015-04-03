using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IBegeleiderRepository
    {
        bool Add(Begeleider begeleider);
        IQueryable<Begeleider> FindAll();
        Begeleider FindByEmail(string hoGentEmail);
        Begeleider FindById(int id);
        void SaveChanges();
        void Update(Begeleider begeleider);
        bool Delete(Begeleider begeleider);
    }
}
