
namespace StageBeheersTool.Models.Domain
{
    public interface IBegeleiderRepository
    {
        Begeleider FindByEmail(string hoGentEmail);
        Begeleider FindById(int id);
        void SaveChanges();
        void Update(Begeleider begeleider, Begeleider model);
        void Add(Begeleider begeleider);
    }
}
