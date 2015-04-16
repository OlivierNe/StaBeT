

using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IStageRepository
    {
        Stage FindById(int id);
        IQueryable<Stage> FindAll();
        IQueryable<Stage> FindAllVanHuidigAcademiejaar();
        void Update(Stage stage);
        void SaveChanges();
    }
}