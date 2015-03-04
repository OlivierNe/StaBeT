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
        IQueryable<Stageopdracht> FindByFilter(int? semester, int? aantalStudenten, string soort, string bedrijf, string locatie);
        IQueryable<Stageopdracht> FindGeldigeStageopdrachten(int? semester, int? aantalStudenten, string soort, string bedrijf, string locatie);
        IQueryable<Stageopdracht> FindStageopdrachtenFrom(Begeleider begeleider);
        void Update(Stageopdracht stageopdracht, Stageopdracht teUpdatenOpdracht);
    }
}