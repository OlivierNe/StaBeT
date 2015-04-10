﻿using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IStageopdrachtRepository
    {
        Stageopdracht FindById(int id);

        IQueryable<Stageopdracht> FindAll();
        IQueryable<Stageopdracht> FindStageopdrachtVoorstellen();
        IQueryable<Stageopdracht> FindGoedgekeurdeStageopdrachten();
        IQueryable<Stageopdracht> FindAfgekeurdeStageopdrachten();
        IQueryable<Stageopdracht> FindBeschikbareStages();
        IQueryable<Stageopdracht> FindToegewezenStageopdrachtenZonderBegeleider();
        IQueryable<Stageopdracht> FindToegewezenStages();
        IQueryable<Stageopdracht> FindAllFromAcademiejaar(string academiejaar);
        IQueryable<Stageopdracht> FindStageopdrachtenVanHuidigeBegeleider();
        IQueryable<Stageopdracht> FindStageopdrachtenVanHuidigBedrijf();

        void Update(Stageopdracht stageopdracht);
        void Delete(Stageopdracht stageopdracht);

        StagebegeleidingAanvraag FindAanvraagById(int id);
        IQueryable<StagebegeleidingAanvraag> FindAllAanvragen();
        IQueryable<StagebegeleidingAanvraag> FindAllAanvragenVan(Begeleider begeleider);

        IQueryable<VoorkeurStage> FindAllStudentVoorkeurenMetIngediendStagedossier();
        VoorkeurStage FindStudentVoorkeurStageByIds(int studentId, int stageId);

        void AddAanvraag(StagebegeleidingAanvraag aanvraag);
        void DeleteAanvraag(StagebegeleidingAanvraag aanvraag);

        string[] FindAllAcademiejaren();
        string[] FindAllAcademiejarenVanBegeleider();
        IQueryable<Stageopdracht> FindAllVanAcademiejaar(string academiejaar);
        IQueryable<Stageopdracht> FindMijnStagesVanAcademiejaar(string academiejaar);

        void SaveChanges();

    }
}