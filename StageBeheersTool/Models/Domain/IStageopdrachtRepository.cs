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
        IQueryable<Stageopdracht> FindByFilter(int? semester, string soort, string bedrijf, string locatie);
        IQueryable<Stageopdracht> FindGeldigeStageopdrachten(int? semester, string soort, string bedrijf, string locatie);
    }
}