using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public interface ISpecialisatieRepository
    {
        IQueryable<Specialisatie> FindAll();
        Specialisatie FindBy(int id);
    }
}