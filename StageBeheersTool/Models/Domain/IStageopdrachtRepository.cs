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
        IQueryable<Stageopdracht> FindById(int id);
        IQueryable<Stageopdracht> FindBy(string seachTerm);
    }
}