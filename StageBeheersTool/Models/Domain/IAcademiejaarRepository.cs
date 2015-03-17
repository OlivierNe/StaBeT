using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public interface IAcademiejaarRepository
    {
        void Add(AcademiejaarInstellingen academiejaar);
        AcademiejaarInstellingen FindByHuidigAcademiejaar();
        AcademiejaarInstellingen FindByAcademiejaar(string academiejaar);
        IQueryable<AcademiejaarInstellingen> FindAll();
        void Update(AcademiejaarInstellingen academiejaar);
        void Delete(AcademiejaarInstellingen academiejaar);
        void SaveChanges();
    }
}