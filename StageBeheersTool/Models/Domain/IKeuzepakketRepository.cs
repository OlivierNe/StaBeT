using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public interface IKeuzepakketRepository
    {
        IQueryable<Keuzepakket> FindAll();
        Keuzepakket FindBy(int id);
    }
}