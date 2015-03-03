using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StageBeheersTool.Models.Domain
{
    public interface IBegeleiderRepository
    {
        Begeleider FindByEmail(string email);
        void SaveChanges();
        void Update(Begeleider begeleider, Begeleider newBegeleider);
        void Add(Begeleider begeleider);
    }
}
