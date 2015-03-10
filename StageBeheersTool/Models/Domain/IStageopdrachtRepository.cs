using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public interface IStageopdrachtRepository
    {
        void Delete(Stageopdracht stageopdracht);
        IQueryable<Stageopdracht> FindAll();
        Stageopdracht FindById(int id);
        Stageopdracht FindGeldigeStageopdrachtById(int id);
        Stageopdracht FindGoedgekeurdeStageById(int id);
        IQueryable<Stageopdracht> FindByFilter(int? semester, int? aantalStudenten, int? specialisatieId, string bedrijf, string locatie);
        IQueryable<Stageopdracht> FindGeldigeStageopdrachten(int? semester, int? aantalStudenten, int? specialisatieId, string bedrijf, string locatie);
        IQueryable<Stageopdracht> FindStageopdrachtenFrom(Begeleider begeleider);
        IQueryable<Stageopdracht> FindGoedgekeurdeStageopdrachten();
        IQueryable<Stageopdracht> FindStageopdrachtVoorstellen();
        IQueryable<Stageopdracht> FindGoedgekeurdeStageopdrachtenByFilter(int? semester, int? aantalStudenten, int? specialisatieId, string bedrijf, string locatie, string student);
        void Update(Stageopdracht stageopdracht, Stageopdracht model);
        void SaveChanges();
    }
}