using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StageBeheersTool.Models.Domain
{
    public interface IBedrijfRepository
    {
        void Add(Bedrijf bedrijf);
        Bedrijf FindByEmail(string email);
        void SaveChanges();
        void DeleteContactpersoon(Contactpersoon contactpersoon);
    }
}
