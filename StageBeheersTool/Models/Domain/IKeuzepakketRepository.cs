using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IKeuzepakketRepository
    {
        IQueryable<Keuzepakket> FindAll();
        Keuzepakket FindBy(int id);
    }
}