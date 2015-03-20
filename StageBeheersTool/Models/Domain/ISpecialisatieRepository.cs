using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface ISpecialisatieRepository
    {
        IQueryable<Specialisatie> FindAll();
        Specialisatie FindBy(int id);
    }
}