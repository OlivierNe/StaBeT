using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StageBeheersTool.Models.Domain
{
    public interface IContactpersoonRepository
    {
        void Delete(Contactpersoon contactpersoon);
        Contactpersoon FindById(int id);
        IQueryable<Contactpersoon> Contactpersonen();
        void SaveChanges();
    }
}
