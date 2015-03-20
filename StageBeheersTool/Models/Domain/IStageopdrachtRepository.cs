using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IStageopdrachtRepository
    {
        void Delete(Stageopdracht stageopdracht);
        IQueryable<Stageopdracht> FindAll();
        Stageopdracht FindById(int id);
        Stageopdracht FindGeldigeStageopdrachtById(int id);
        Stageopdracht FindGoedgekeurdeStageById(int id);
        IQueryable<Stageopdracht> FindAllByFilter(int? semester, int? aantalStudenten, int? specialisatieId, string bedrijf, string locatie, string student);
        IQueryable<Stageopdracht> FindByFilter(int? semester, int? aantalStudenten, int? specialisatieId, string bedrijf, string locatie);
        IQueryable<Stageopdracht> FindGeldigeStageopdrachten(int? semester, int? aantalStudenten, int? specialisatieId, string bedrijf, string locatie);
        IQueryable<Stageopdracht> FindStageopdrachtenFrom(Begeleider begeleider);
        IQueryable<Stageopdracht> FindGoedgekeurdeStageopdrachten();
        IQueryable<Stageopdracht> FindStageopdrachtVoorstellen();
        IQueryable<Stageopdracht> FindGoedgekeurdeStageopdrachtenByFilter(int? semester, int? aantalStudenten, int? specialisatieId, string bedrijf, string locatie, string student);
        void Update(Stageopdracht stageopdracht);
        void SaveChanges();
        StageBegeleidAanvraag FindAanvraagById(int id);
        void AddAanvraag(StageBegeleidAanvraag aanvraag);
        void DeleteAanvraag(StageBegeleidAanvraag aanvraag);
        IQueryable<StageBegeleidAanvraag> FindAllAanvragen();
        IQueryable<StageBegeleidAanvraag> FindAllAanvragenFrom(Begeleider begeleider);
        
    }
}